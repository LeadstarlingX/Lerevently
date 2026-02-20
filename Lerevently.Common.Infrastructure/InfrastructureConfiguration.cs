using Lerevently.Common.Application.Caching;
using Lerevently.Common.Application.Clock;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.EventBus;
using Lerevently.Common.Infrastructure.Authentication;
using Lerevently.Common.Infrastructure.Caching;
using Lerevently.Common.Infrastructure.Clock;
using Lerevently.Common.Infrastructure.Data;
using Lerevently.Common.Infrastructure.Interceptors;
using MassTransit;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using StackExchange.Redis;

namespace Lerevently.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers,
        IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("Database")!;
        
        var npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.TryAddSingleton<PublishDomainEventsInterceptor>();

        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();


        services.AddAuthenticationInternal();
        
        /// To solve the problem of not being able to resolve the connection multiplexer when
        /// running the migrations, at real runtime the first part will always work.
        try
        {
            var cacheConnectionString = configuration.GetConnectionString("Cache");
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(cacheConnectionString!);
            services.TryAddSingleton(connectionMultiplexer);
            services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));

        }
        catch
        {
            services.AddDistributedMemoryCache();
        }

        services.TryAddSingleton<ICacheService, CacheService>();
        
        services.TryAddSingleton<IEventBus, EventBus.EventBus>();
        
        services.AddMassTransit(configure =>
        {
            foreach (var configureConsumer in moduleConfigureConsumers)
            {
                configureConsumer(configure);
            }    
            
            configure.SetKebabCaseEndpointNameFormatter();
            
            configure.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }
    
    
}