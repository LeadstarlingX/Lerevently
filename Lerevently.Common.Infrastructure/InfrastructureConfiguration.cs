using Lerevently.Common.Application.Caching;
using Lerevently.Common.Application.Clock;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Infrastructure.Caching;
using Lerevently.Common.Infrastructure.Clock;
using Lerevently.Common.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using StackExchange.Redis;

namespace Lerevently.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("Database")!;
        
        var npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();

        var cacheConnectionString = configuration.GetConnectionString("Cache");
        IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(cacheConnectionString!);
        services.TryAddSingleton(connectionMultiplexer);
        services.AddStackExchangeRedisCache(options => 
            options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        
        services.TryAddSingleton<ICacheService, CacheService>();

        
        return services;
    }
}