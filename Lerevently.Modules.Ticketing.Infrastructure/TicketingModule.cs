using Lerevently.Common.Infrastructure.Interceptors;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Ticketing.Application.Abstractions.Data;
using Lerevently.Modules.Ticketing.Application.Abstractions.Payments;
using Lerevently.Modules.Ticketing.Application.Carts;
using Lerevently.Modules.Ticketing.Domain.Customers;
using Lerevently.Modules.Ticketing.Domain.Events;
using Lerevently.Modules.Ticketing.Domain.Orders;
using Lerevently.Modules.Ticketing.Domain.Payments;
using Lerevently.Modules.Ticketing.Domain.Tickets;
using Lerevently.Modules.Ticketing.Infrastructure.Customers;
using Lerevently.Modules.Ticketing.Infrastructure.Database;
using Lerevently.Modules.Ticketing.Infrastructure.Events;
using Lerevently.Modules.Ticketing.Infrastructure.Orders;
using Lerevently.Modules.Ticketing.Infrastructure.Payments;
using Lerevently.Modules.Ticketing.Infrastructure.Tickets;
using Lerevently.Modules.Ticketing.Presentation.Customers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.Modules.Ticketing.Infrastructure;

public static class TicketingModule
{
    public static IServiceCollection AddTicketingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        registrationConfigurator.AddConsumer<UserRegisteredIntegrationEventConsumer>();
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TicketingDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Ticketing))
                .AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>()));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TicketingDbContext>());

        services.AddSingleton<CartService>();
        services.AddSingleton<IPaymentService, PaymentService>();
    }
}
