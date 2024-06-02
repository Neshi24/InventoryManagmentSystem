using System.Text;
using AutoMapper;
using CommonPackage;
using EasyNetQ;
using OrderService.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.Repo;
using OrderService.Services;
using OpenTelemetry.Trace;
using Shared;
using DbContext = OrderService.Repo.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8082");
var serviceName = "OrderService";
var serviceVersion = "1.0.0";
builder.Services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb")));

var config = new MapperConfiguration(conf =>
{
    conf.CreateMap<OrderDto, Order>();
});
var connectionStr = "amqp://guest:guest@rabbitmq";
var hostname = "rabbitmq"; // Hostname for RabbitMQ connection

// Register the MessageClient with the DI container
builder.Services.AddSingleton<MessageClient>(sp =>
    new MessageClient(RabbitHutch.CreateBus(connectionStr), hostname));
builder.Services.AddHostedService<MessageHandler>();
builder.Services.AddSingleton(config.CreateMapper());
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();
builder.Services.AddControllers();

// Configure authentication
var secretKey = builder.Configuration.GetValue<string>("AppSettings:Token");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), 
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