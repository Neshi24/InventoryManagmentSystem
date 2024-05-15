
using AuthService.Data;
using AuthService.Data.Interface;
using AuthService.Data.Repositories;
using AuthService.Model;
using AuthService.Services.Implementations;
using AuthService.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8081");



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Configure AutoMapper
var config = new MapperConfiguration(conf =>
{
    conf.CreateMap<UserDto, User>();
});
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

// Configure DbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDatabase")));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();