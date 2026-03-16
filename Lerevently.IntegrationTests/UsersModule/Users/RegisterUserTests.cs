using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Lerevently.IntegrationTests.Abstractions;

namespace Lerevently.IntegrationTests.UsersModule.Users;

public class RegisterUserTests : BaseIntegrationTest
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
    [MethodDataSource(typeof(TestDataSources), nameof(TestDataSources.AdditionTestData))]
    public async Task Should_ReturnBadRequest_WhenRequestIsNotValid(
        string email,
        string password,
        string firstName,
        string lastName)
    {
        // Arrange
        var request = new Modules.Users.Presentation.Users.RegisterUser.Request
        {
            Email = email,
            Password = password,
            FirstName = firstName,
            LastName = lastName
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Should_ReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var request = new Modules.Users.Presentation.Users.RegisterUser.Request
        {
            Email = "create@test.com",
            Password = Faker.Internet.Password(),
            FirstName = Faker.Name.FirstName(),
            LastName = Faker.Name.LastName()
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task Should_ReturnAccessToken_WhenUserIsRegistered()
    {
        // Arrange
        var request = new Modules.Users.Presentation.Users.RegisterUser.Request
        {
            Email = "token@test.com",
            Password = Faker.Internet.Password(),
            FirstName = Faker.Name.FirstName(),
            LastName = Faker.Name.LastName()
        };

        await _httpClient.PostAsJsonAsync("users/register", request);

        // Act
        var accessToken = await GetAccessTokenAsync(request.Email, request.Password);

        // Assert
        accessToken.Should().NotBeEmpty();
    }

    public static class TestDataSources
    {
        public static IEnumerable<Func<(string, string, string, string)>> AdditionTestData()
        {
            yield return () => ("", Faker.Internet.Password(), Faker.Name.FirstName(), Faker.Name.LastName());
            yield return () => ($"user-{Guid.NewGuid()}@test.com", "", Faker.Name.FirstName(), Faker.Name.LastName());
            yield return () =>
                ($"user-{Guid.NewGuid()}@test.com", "12345", Faker.Name.FirstName(), Faker.Name.LastName());
            yield return () =>
                ($"user-{Guid.NewGuid()}@test.com", Faker.Internet.Password(), "", Faker.Name.LastName());
            yield return () =>
                ($"user-{Guid.NewGuid()}@test.com", Faker.Internet.Password(), Faker.Name.FirstName(), "");
        }
    }
}