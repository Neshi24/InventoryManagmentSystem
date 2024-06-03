using AutoMapper;
using CommonPackage;
using EasyNetQ;
using OrderService.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using OrderService.Repo;
using OrderService.Services;
using OpenTelemetry.Trace;
using Shared;
using DbContext = OrderService.Repo.DbContext;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8082");
var serviceName = "OrderService";
var serviceVersion = "1.0.0";
string sqlServer = Environment.GetEnvironmentVariable("sqlServer") ?? string.Empty;
string sqlPort = Environment.GetEnvironmentVariable("sqlPort") ?? string.Empty;
string sqlUser = Environment.GetEnvironmentVariable("sqlUser") ?? string.Empty;
string database = Environment.GetEnvironmentVariable("database") ?? string.Empty;
string sqlPass = Environment.GetEnvironmentVariable("SA_PASSWORD") ?? string.Empty;
string rmqUser = Environment.GetEnvironmentVariable("rmqUser") ?? string.Empty;
string rmqPass = Environment.GetEnvironmentVariable("rmqPass") ?? string.Empty;
builder.Services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));

string connectionString = $"Server={sqlServer},{sqlPort};Database={database};User={sqlUser};Password={sqlPass};TrustServerCertificate=true;";
builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(connectionString));

var config = new MapperConfiguration(conf =>
{
    conf.CreateMap<OrderDto, Order>();
});

var connectionStr = $"amqp://{rmqUser}:{rmqPass}@rabbitmq"; // Connection string for RabbitMQ
var hostname = "rabbitmq"; // Hostname for RabbitMQ connection

// Register the MessageClient with the DI container
builder.Services.AddSingleton<MessageClient>(sp =>
    new MessageClient(RabbitHutch.CreateBus(connectionStr), hostname));
builder.Services.AddHostedService<MessageHandler>();
builder.Services.AddSingleton(config.CreateMapper());
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();
builder.Services.AddControllers();
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