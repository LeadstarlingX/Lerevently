namespace Lerevently.Modules.Ticketing.Infrastructure.DataSeeder.Contracts;

public sealed record UserSeedData(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);

public sealed record EventSeedData(
    Guid Id,
    string Title,
    string Description,
    string Location,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    int Status,
    Guid CategoryId
);

public sealed record TicketTypeSeedData(
    Guid Id,
    Guid EventId,
    string Name,
    decimal Price,
    string Currency,
    decimal Quantity
);