using System.Net.Http.Json;
using AuthService.Model;
using AuthService.Tests.DBsetup;
using FluentAssertions;



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