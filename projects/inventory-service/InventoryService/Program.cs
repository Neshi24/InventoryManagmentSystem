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
using DbContext = InventoryService.Repo.DbContext;


var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8081");
var serviceName = "InventoryService";
var serviceVersion = "1.0.0";
builder.Services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InventoryDb")));

var config = new MapperConfiguration(conf =>
{
    conf.CreateMap<ItemDto, Item>();
});
var connectionStr = "amqp://guest:guest@rabbitmq";
var hostname = "rabbitmq";

builder.Services.AddSingleton(new MessageClient(RabbitHutch.CreateBus(connectionStr), hostname));
builder.Services.AddHostedService<MessageHandler>();
builder.Services.AddSingleton(config.CreateMapper());
builder.Services.AddScoped<IItemRepo, ItemRepo>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddControllers();

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

app.MapControllers();

await app.RunAsync();
