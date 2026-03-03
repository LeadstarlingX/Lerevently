using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Lerevently.Modules.Events.IntegrationTests.Abstractions;
using Lerevently.Modules.Users.Application.Users.GetUser;
using Lerevently.Modules.Users.Infrastructure.Database;
using Lerevently.Modules.Users.Infrastructure.Identity;
using Lerevently.Modules.Users.Presentation.Users;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lerevently.Modules.Events.IntegrationTests.Users;

public class GetUserProfileTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    protected ISender Sender;
    private static KeyCloakOptions _options;
    protected UsersDbContext DbContext;
    
    [Before(Test)]
    public async Task SetupTest()
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        _options = _scope.ServiceProvider.GetRequiredService<IOptions<KeyCloakOptions>>().Value;
        DbContext = _scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        // Fresh DbContext, clean state
        DbContext.Users.RemoveRange(DbContext.Users);
        await DbContext.SaveChangesAsync();
    }
    
    
    [After(Test)]
    public async ValueTask TeardownTest()
    {
        if (_scope is IAsyncDisposable asyncScope)
            await asyncScope.DisposeAsync();
        else
            _scope.Dispose();
    }

    [Test]
    public async Task Should_ReturnUnauthorized_WhenAccessTokenNotProvided()
    {
        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Should_ReturnOk_WhenUserExists()
    {
        // Arrange
        string accessToken = await RegisterUserAndGetAccessTokenAsync("exists@test.com", Faker.Internet.Password());
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        UserResponse? user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
    }

    private async Task<string> RegisterUserAndGetAccessTokenAsync(string email, string password)
    {
        var request = new RegisterUser.Request
        {
            Email = email,
            Password = password,
            FirstName = Faker.Name.FirstName(),
            LastName = Faker.Name.LastName()
        };

        await HttpClient.PostAsJsonAsync("users/register", request);

        string accessToken = await GetAccessTokenAsync(request.Email, request.Password);

        return accessToken;
    }
}