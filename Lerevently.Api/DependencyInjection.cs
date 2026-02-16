namespace Lerevently.Api;

internal static class DependencyInjection
{
    
    
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddOpenApi()
            .AddApiSwagger()
            .AddEndpointsApiExplorer();

        return services;
    }
    
    private static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => { options.CustomSchemaIds(t => t.FullName?.Replace("+", ".")); });

        return services;
    }

    private static IServiceCollection AddModulesConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {

        return services;
    }
    
    
}