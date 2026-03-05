using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Bogus;
using Lerevently.Modules.Users.Infrastructure.Database;
using Lerevently.Modules.Users.Infrastructure.Identity;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lerevently.Modules.Users.IntegrationTests.Abstractions;

public class BaseIntegrationTest : IAsyncDisposable
{
    protected static readonly Faker Faker = new();
    private static IServiceScope _scope;
    protected static ISender Sender;
    protected static HttpClient HttpClient;
    private static KeyCloakOptions _options;
    protected static UsersDbContext DbContext;
    
    [ClassDataSource<IntegrationTestWebAppFactory>(Shared = SharedType.PerAssembly)]
    public static IntegrationTestWebAppFactory factory { get; set; }

    
    [Before(TestSession)]
    public static void Setup()
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        HttpClient = factory.CreateClient();
        _options = _scope.ServiceProvider.GetRequiredService<IOptions<KeyCloakOptions>>().Value;
        DbContext = _scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        
    }


    protected async Task<string> GetAccessTokenAsync(string email, string password)
    {
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

        using HttpResponseMessage authorizationResponse = await client.SendAsync(authRequest);

        authorizationResponse.EnsureSuccessStatusCode();

        AuthToken authToken = await authorizationResponse.Content.ReadFromJsonAsync<AuthToken>();

        return authToken!.AccessToken;
    }

    internal sealed class AuthToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (_scope is IAsyncDisposable asyncScope)
        {
            await asyncScope.DisposeAsync();
        }
        else
        {
            _scope.Dispose();
        }
    }
}
