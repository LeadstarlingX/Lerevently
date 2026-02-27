namespace Lerevently.Modules.Ticketing.Infrastructure.DataSeeder.Contracts;

public sealed record CustomerSeedData(
    Guid Id,
    string Email,
    string FirstName,
    string LastName);

public sealed record EventProjectionSeedData(
    Guid Id,
    string Title,
    string Description,
    string Location,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    bool Canceled
);

public sealed record TicketTypeProjectionSeedData(
    Guid Id,
    Guid EventId,
    string Name,
    decimal Price,
    string Currency,
    decimal Quantity,
    decimal AvailableQuantity
);

public sealed record OrderSeedData(
    Guid Id,
    Guid CustomerId,
    int Status,
    decimal TotalPrice,
    string Currency,
    bool TicketsIssued,
    DateTime CreatedAtUtc
);

public sealed record OrderItemSeedData(
    Guid Id,
    Guid OrderId,
    Guid TicketTypeId,
    decimal Quantity,
    decimal UnitPrice,
    decimal Price,
    string Currency
);

public sealed record TicketSeedData(
    Guid Id,
    Guid CustomerId,
    Guid OrderId,
    Guid EventId,
    Guid TicketTypeId,
    string Code,
    DateTime CreatedAtUtc,
    bool Archived
);

public sealed record PaymentSeedData(
    Guid Id,
    Guid OrderId,
    Guid TransactionId,
    decimal Amount,
    string Currency,
    decimal? AmountRefunded,
    DateTime CreatedAtUtc,
    DateTime? RefundedAtUtc
);