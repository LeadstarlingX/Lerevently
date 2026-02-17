using System.Reflection;
using FluentValidation;
using Lerevently.Common.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.Common.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services,
        Assembly[] moduleAssemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(moduleAssemblies);
            
            config.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));
            
            config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(moduleAssemblies, includeInternalTypes: true);


        return services;
    }
}