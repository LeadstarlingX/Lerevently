using Lerevently.Api.Extensions;
using Lerevently.Api.Middleware;
using Lerevently.Common.Infrastructure.EventBus;
using RabbitMQ.Client;

namespace Lerevently.Api;

internal static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddApiSwagger();
        services.AddEndpointsApiExplorer();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddMyHealthChecks(configuration);

        return services;
    }

    private static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => { options.CustomSchemaIds(t => t.FullName?.Replace("+", ".")); });

        return services;
    }

    private static IServiceCollection AddMyHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {

        var temp = new RabbitMqSettings(configuration.GetConnectionString("Queue")!);

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Database")!)
            .AddRedis(configuration.GetConnectionString("Cache")!)
            .AddRabbitMQ(sp =>
            {
                var factory = new ConnectionFactory { Uri = new Uri(temp.Host) };
                return factory.CreateConnectionAsync();
            })
            .AddUrlGroup(new Uri(configuration["KeyCloak:HealthUrl"]!), HttpMethod.Get, "keycloak");

        return services;
    }
}