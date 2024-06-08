using System.Net.Http.Json;
using AuthService.Data;
using AuthService.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
           
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            db.Database.EnsureCreated();
        });
    }
}

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnUser()
    {
        // Arrange
        var userDto = new UserDto
        {
            Username = "testuser",
            Email = "testuser@example.com",
            password = "Test@1234"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/Auth/Register", userDto);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with status code {response.StatusCode} and message: {errorContent}");
        }

        var createdUser = await response.Content.ReadFromJsonAsync<User>();
        createdUser.Should().NotBeNull();
        createdUser.Username.Should().Be(userDto.Username);
        createdUser.Email.Should().Be(userDto.Email);
    }
}