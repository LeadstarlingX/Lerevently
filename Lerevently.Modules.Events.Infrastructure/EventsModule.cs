using Lerevently.Common.Infrastructure.Outbox;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Events.Domain.Categories;
using Lerevently.Modules.Events.Domain.Events;
using Lerevently.Modules.Events.Domain.TicktTypes;
using Lerevently.Modules.Events.Infrastructure.Categories;
using Lerevently.Modules.Events.Infrastructure.Database;
using Lerevently.Modules.Events.Infrastructure.Events;
using Lerevently.Modules.Events.Infrastructure.Outbox;
using Lerevently.Modules.Events.Infrastructure.TicketTypes;
using Lerevently.Modules.Events.Presentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IUnitOfWork = Lerevently.Modules.Events.Application.Abstractions.Data.IUnitOfWork;

namespace Lerevently.Modules.Events.Infrastructure;

public static class EventsModule
{
    public static IServiceCollection AddEventsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpoints(AssemblyReference.Assembly);

        services.AddInfrastructure(configuration);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("Database")!;


        services.AddDbContext<EventsDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    databaseConnectionString,
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Events))
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));
        
        services.Configure<OutboxOptions>(configuration.GetSection("Events:Outbox"));

        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<EventsDbContext>());

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
    }
}