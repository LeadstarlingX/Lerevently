using Lerevently.Api;
using Lerevently.Modules.Users.Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using TUnit.Core.Interfaces;

namespace Lerevently.IntegrationTests.Abstractions;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    // Obsolete constructors will be removed in future versions, keep the parameter constructor
    //  it's enough to pass the image.

    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder("postgres:18.0")
            .WithDatabase("lerevently")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private readonly KeycloakContainer _keycloakContainer =
        new KeycloakBuilder("keycloak/keycloak:26.4")
            .WithResourceMapping(
                new FileInfo("lerevently-realm-export.json"),
                new FileInfo("/opt/keycloak/data/import/realm.json"))
            .WithCommand("--import-realm")
            .Build();

    private readonly RedisContainer _redisContainer =
        new RedisBuilder("redis:8.4.0")
            .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _keycloakContainer.StartAsync();

        // Force the host to start and apply migrations before any tests run
        using var _ = CreateClient();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:Database", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:Cache", _redisContainer.GetConnectionString());

        var keycloakAddress = _keycloakContainer.GetBaseAddress();
        var keyCloakRealmUrl = $"{keycloakAddress}realms/lerevently";

        Environment.SetEnvironmentVariable(
            "Authentication:MetadataAddress",
            $"{keyCloakRealmUrl}/.well-known/openid-configuration");
        Environment.SetEnvironmentVariable(
            "Authentication:TokenValidationParameters:ValidIssuer",
            keyCloakRealmUrl);

        builder.ConfigureTestServices(services =>
        {
            services.Configure<KeyCloakOptions>(o =>
            {
                o.AdminUrl = $"{keycloakAddress}admin/realms/lerevently/";
                o.TokenUrl = $"{keyCloakRealmUrl}/protocol/openid-connect/token";
                o.ConfidentialClientId = "lerevently-confidential-client";
                o.ConfidentialClientSecret = "7gaIbiPBBnY0dzIeBosoG7zM5J1y9AVQ";
            });
        });
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        await _keycloakContainer.StopAsync();
    }
}