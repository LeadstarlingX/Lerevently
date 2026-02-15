using System.Reflection.Metadata;
using FluentValidation;
using Lerevently.Modules.Events.Application.Abstractions.Data;
using Lerevently.Modules.Events.Application.Events;
using Lerevently.Modules.Events.Domain.Events;
using Lerevently.Modules.Events.Infrastructure.Data;
using Lerevently.Modules.Events.Infrastructure.Database;
using Lerevently.Modules.Events.Infrastructure.Events;
using Lerevently.Modules.Events.Presentation.Events;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace Lerevently.Modules.Events.Infrastructure
{
    public static class EventsModule
    {
        public static void MapEndpoints(IEndpointRouteBuilder app)
        {
            EventEndpoints.MapEndpoints(app);
        }


        public static IServiceCollection AddEventsModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(Application.AssemblyReference.Assembly));
            
            services.AddValidatorsFromAssembly(Application.AssemblyReference.Assembly, includeInternalTypes: true);
            
            services.AddInfrastructure(configuration);
            
            return services;
        }
        
        private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            string databaseConnectionString = configuration.GetConnectionString("Database")!;
            
            NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
            services.TryAddSingleton(npgsqlDataSource);
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
            
            
            services.AddDbContext<EventsDbContext>(options =>
            {
                options.UseNpgsql(databaseConnectionString,
                    npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Events));
            });

            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<EventsDbContext>());
            
        }
        
    }
}
