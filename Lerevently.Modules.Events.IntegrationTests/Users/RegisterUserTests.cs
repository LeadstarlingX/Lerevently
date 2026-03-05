using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Lerevently.Modules.Events.IntegrationTests.Abstractions;
using Lerevently.Modules.Users.Infrastructure.Database;
using Lerevently.Modules.Users.Infrastructure.Identity;
using Lerevently.Modules.Users.Presentation.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TUnit.Core;

namespace Lerevently.Modules.Events.IntegrationTests.Users;

public class RegisterUserTests : BaseIntegrationTest
{

    public static class TestDataSources
    {
        public static IEnumerable<Func<(string, string, string, string)>> AdditionTestData()
        {
            yield return () => ("", Faker.Internet.Password(), Faker.Name.FirstName(), Faker.Name.LastName());
            yield return () => (Faker.Internet.Email(), "", Faker.Name.FirstName(), Faker.Name.LastName() );
            yield return () => (Faker.Internet.Email(), "12345", Faker.Name.FirstName(), Faker.Name.LastName());
            yield return () => (Faker.Internet.Email(), Faker.Internet.Password(), "", Faker.Name.LastName());
            yield return () => (Faker.Internet.Email(), Faker.Internet.Password(), Faker.Name.FirstName(), "" );
        }
    }
    
    private IServiceScope _scope;
    
    [Before(Test)]
    public async Task SetupTest()
    {
        _scope = factory.Services.CreateScope();
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
    [MethodDataSource(typeof(TestDataSources), nameof(TestDataSources.AdditionTestData))]
    public async Task Should_ReturnBadRequest_WhenRequestIsNotValid(
        string email,
        string password,
        string firstName,
        string lastName)
    {
        // Arrange
        var request = new RegisterUser.Request
        {
            Email = email,
            Password = password,
            FirstName = firstName,
            LastName = lastName
        };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Should_ReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterUser.Request
        {
            Email = "create@test.com",
            Password = Faker.Internet.Password(),
            FirstName = Faker.Name.FirstName(),
            LastName = Faker.Name.LastName()
        };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task Should_ReturnAccessToken_WhenUserIsRegistered()
    {
        // Arrange
        var request = new RegisterUser.Request
        {
            Email = "token@test.com",
            Password = Faker.Internet.Password(),
            FirstName = Faker.Name.FirstName(),
            LastName = Faker.Name.LastName()
        };

        await HttpClient.PostAsJsonAsync("users/register", request);

        // Act
        string accessToken = await GetAccessTokenAsync(request.Email, request.Password);

        // Assert
        accessToken.Should().NotBeEmpty();
    }
}
