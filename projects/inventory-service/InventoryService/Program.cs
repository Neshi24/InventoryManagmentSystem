using AutoMapper;
using InventoryService.Models;
using InventoryService.Repo;
using InventoryService.Services;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080");

// Add AutoMapper
var config = new MapperConfiguration(conf =>
{
    conf.CreateMap<ItemDto, Item>();
   
});
builder.Services.AddSingleton(config.CreateMapper());

builder.Services.AddScoped<IItemRepo, ItemRepo>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();