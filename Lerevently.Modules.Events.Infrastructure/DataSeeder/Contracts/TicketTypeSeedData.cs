namespace Lerevently.Modules.Events.Infrastructure.DataSeeder.Contracts;

public sealed record TicketTypeSeedData(
    Guid Id,
    Guid EventId,
    string Name,
    decimal Price,
    string Currency,
    decimal Quantity
);