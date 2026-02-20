namespace Lerevently.Modules.Events.Infrastructure.DataSeeder.Contracts;

internal sealed record EventSeedData(
    Guid Id,
    string Title,
    string Description,
    string Location,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    int Status
);