using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Infrastructure.Outbox;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Ticketing.Application.Abstractions.Authentication;
using Lerevently.Modules.Ticketing.Application.Abstractions.Data;
using Lerevently.Modules.Ticketing.Application.Abstractions.Payments;
using Lerevently.Modules.Ticketing.Application.Carts;
using Lerevently.Modules.Ticketing.Domain.Customers;
using Lerevently.Modules.Ticketing.Domain.Events;
using Lerevently.Modules.Ticketing.Domain.Orders;
using Lerevently.Modules.Ticketing.Domain.Payments;
using Lerevently.Modules.Ticketing.Domain.Tickets;
using Lerevently.Modules.Ticketing.Infrastructure.Authentication;
using Lerevently.Modules.Ticketing.Infrastructure.Customers;
using Lerevently.Modules.Ticketing.Infrastructure.Database;
using Lerevently.Modules.Ticketing.Infrastructure.Events;
using Lerevently.Modules.Ticketing.Infrastructure.Orders;
using Lerevently.Modules.Ticketing.Infrastructure.Outbox;
using Lerevently.Modules.Ticketing.Infrastructure.Payments;
using Lerevently.Modules.Ticketing.Infrastructure.Tickets;
using Lerevently.Modules.Ticketing.Presentation;
using Lerevently.Modules.Ticketing.Presentation.Customers;
using Lerevently.Modules.Ticketing.Presentation.Events;
using Lerevently.Modules.Ticketing.Presentation.TicketTypes;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lerevently.Modules.Ticketing.Infrastructure;

public static class TicketingModule
{
    public static IServiceCollection AddTicketingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDomainEventHandlers();
        
        services.AddInfrastructure(configuration);

        services.AddEndpoints(AssemblyReference.Assembly);

        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        registrationConfigurator.AddConsumer<UserRegisteredIntegrationEventConsumer>();
        registrationConfigurator.AddConsumer<UserProfileUpdatedIntegrationEventConsumer>();
        registrationConfigurator.AddConsumer<EventPublishedIntegrationEventConsumer>();
        registrationConfigurator.AddConsumer<TicketTypePriceChangedIntegrationEventConsumer>();
    }


    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TicketingDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Ticketing))
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));
        
        
        services.Configure<OutboxOptions>(configuration.GetSection("Ticketing:Outbox"));

        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddScoped<ICustomerContext, CustomerContext>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TicketingDbContext>());

        services.AddSingleton<CartService>();
        services.AddSingleton<IPaymentService, PaymentService>();
    }
    
    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        Type[] domainEventHandlers = Application.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
            .ToArray();

        foreach (Type domainEventHandler in domainEventHandlers)
        {
            services.TryAddScoped(domainEventHandler);

            Type domainEvent = domainEventHandler
                .GetInterfaces()
                .Single(i => i.IsGenericType)
                .GetGenericArguments()
                .Single();

            Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

            services.Decorate(domainEventHandler, closedIdempotentHandler);
        }
    }
}