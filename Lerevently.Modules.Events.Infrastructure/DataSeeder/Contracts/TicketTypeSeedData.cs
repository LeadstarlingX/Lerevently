namespace Lerevently.Modules.Events.Infrastructure.DataSeeder.Contracts;

internal sealed record TicketTypeSeedData(
    Guid Id,
    Guid EventId,
    string Name,
    decimal Price,
    string Currency,
    decimal Quantity
);