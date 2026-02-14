using Lerevently.Modules.Events.Api.Database;
using Lerevently.Modules.Events.Api.Events;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lerevently.Modules.Events.Api
{
    public static class EventsModule
    {
        public static void MapEndpoints(IEndpointRouteBuilder app)
        {
            GetEvent.MapEndpoint(app);
            CreateEvent.MapEndpoint(app);
        }


        public static IServiceCollection AddEventsModule(this IServiceCollection services, IConfiguration configuration)
        {
            string databaseConnectionString = configuration.GetConnectionString("Database")!;

            services.AddDbContext<Database.EventsDbContext>(options =>
            {
                options.UseNpgsql(databaseConnectionString,
                    npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Events));
            });

            return services;
        }
    }
}
