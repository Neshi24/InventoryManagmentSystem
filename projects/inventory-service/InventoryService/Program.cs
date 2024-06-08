using System.Text;
using AutoMapper;
using CommonPackage;
using EasyNetQ;
using InventoryService.RabbitMQ;
using InventoryService.Repo;
using InventoryService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Trace;
using Shared;
using InventoryService.Middleware;
using DbContext = InventoryService.Repo.DbContext;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8081");
var serviceName = "InventoryService";
var serviceVersion = "1.0.0";
string sqlServer = Environment.GetEnvironmentVariable("sqlServer") ?? string.Empty;
string sqlPort = Environment.GetEnvironmentVariable("sqlPort") ?? string.Empty;
string sqlUser = Environment.GetEnvironmentVariable("sqlUser") ?? string.Empty;
string database = Environment.GetEnvironmentVariable("database") ?? string.Empty;
string sqlPass = Environment.GetEnvironmentVariable("SA_PASSWORD") ?? string.Empty;
string rmqUser = Environment.GetEnvironmentVariable("rmqUser") ?? string.Empty;
string rmqPass = Environment.GetEnvironmentVariable("rmqPass") ?? string.Empty;
string rmqExchange = Environment.GetEnvironmentVariable("rmqExchange") ?? string.Empty;

builder.Services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));

string connectionString = $"Server={sqlServer},{sqlPort};Database={database};User={sqlUser};Password={sqlPass};TrustServerCertificate=true;";
Console.WriteLine($"Connection string: {connectionString}");

builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(connectionString));

var config = new MapperConfiguration(conf =>
{
    conf.CreateMap<ItemDto, Item>();
});
var connectionStr = $"amqp://{rmqUser}:{rmqPass}@rabbitmq";
Console.WriteLine($"Connection string: {connectionStr}");
var hostname = "rabbitmq";

builder.Services.AddSingleton(new MessageClient(RabbitHutch.CreateBus(connectionStr), hostname, rmqExchange));
builder.Services.AddHostedService<MessageHandler>();
builder.Services.AddSingleton(config.CreateMapper());
builder.Services.AddScoped<IItemRepo, ItemRepo>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddControllers();
builder.Services.AddSingleton<PerformanceMonitorService>();

// Configure JWT authentication
var secretKey = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:Token"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Log the authentication failure
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // Log the success of token validation
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            },
        };
    });

// Add authorization services
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Your API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<PerformanceMonitoringMiddleware>();

app.MapControllers();

await app.RunAsync();
