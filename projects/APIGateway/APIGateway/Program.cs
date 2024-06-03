using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8085");
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("ocelot.json", false, false);

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseOcelot().Wait();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();