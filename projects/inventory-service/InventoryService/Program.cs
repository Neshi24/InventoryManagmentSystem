using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryService.Repo;
using InventoryService.Services;
using Shared;
using DbContext = InventoryService.Repo.DbContext;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8081");
builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InventoryDb")));

var config = new MapperConfiguration(conf =>
{
    conf.CreateMap<ItemDto, Item>();
});
builder.Services.AddSingleton(config.CreateMapper());
builder.Services.AddScoped<IItemRepo, ItemRepo>();
builder.Services.AddScoped<IItemService, ItemService>();
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