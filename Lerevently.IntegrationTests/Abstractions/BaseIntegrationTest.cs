using Bogus;
using Lerevently.Modules.Users.Infrastructure.Database;
using Lerevently.Modules.Users.Infrastructure.Identity;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lerevently.IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest : IAsyncDisposable
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
