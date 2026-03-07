using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Bogus;
using Lerevently.Modules.Users.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lerevently.IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest
{
    protected const int TimeForSpan = 30;
    protected static readonly Faker Faker = new();
    

    [ClassDataSource<IntegrationTestWebAppFactory>(Shared = SharedType.PerAssembly)]
    public static IntegrationTestWebAppFactory factory { get; set; }


    protected async Task<string> GetAccessTokenAsync(string email, string password)
    {
        using var scope = factory.Services.CreateScope();
        var _options = scope.ServiceProvider
            .GetRequiredService<IOptions<KeyCloakOptions>>().Value;

        using var client = new HttpClient();

        var authRequestParameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _options.PublicClientId),
            new("scope", "openid"),
            new("grant_type", "password"),
            new("username", email),
            new("password", password)
        };

        using var authRequestContent = new FormUrlEncodedContent(authRequestParameters);

        using var authRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_options.TokenUrl));
        authRequest.Content = authRequestContent;

        using var authorizationResponse = await client.SendAsync(authRequest);

        authorizationResponse.EnsureSuccessStatusCode();

        var authToken = await authorizationResponse.Content.ReadFromJsonAsync<AuthToken>();

        return authToken!.AccessToken;
    }

    internal sealed class AuthToken
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; init; }
    }
}