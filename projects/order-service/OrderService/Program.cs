using AutoMapper;
using CommonPackage;
using EasyNetQ;
using OrderService.Repo;
using OrderService.Services;
using Shared;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using OrderService.RabbitMQ;
using DbContext = OrderService.Repo.DbContext;


var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8082");
var serviceName = "InventoryService";
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

builder.Services.AddSingleton(new MessageClient(RabbitHutch.CreateBus(connectionStr)));
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