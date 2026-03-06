using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Users.Application.Users.GetUser;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Lerevently.IntegrationTests.UsersModule.Users;

public class GetUserProfileTests : BaseIntegrationTest
{
    private HttpClient _httpClient;

    [Before(Test)]
    public async Task SetupTest()
    {
        _httpClient = factory.CreateClient();
    }


    [After(Test)]
    public async ValueTask TeardownTest()
    {
        _httpClient.Dispose();
    }

    [Test]
    public async Task Should_ReturnUnauthorized_WhenAccessTokenNotProvided()
    {
        // Act
        var response = await _httpClient.GetAsync("users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Should_ReturnOk_WhenUserExists()
    {
        // Arrange
        var accessToken = await RegisterUserAndGetAccessTokenAsync("exists@test.com", Faker.Internet.Password());
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);

        // Act
        var response = await _httpClient.GetAsync("users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
    }

    private async Task<string> RegisterUserAndGetAccessTokenAsync(string email, string password)
    {
        var request = new Modules.Users.Presentation.Users.RegisterUser.Request
        {
            Email = email,
            Password = password,
            FirstName = Faker.Name.FirstName(),
            LastName = Faker.Name.LastName()
        };

        await _httpClient.PostAsJsonAsync("users/register", request);

        var accessToken = await GetAccessTokenAsync(request.Email, request.Password);

        return accessToken;
    }
}