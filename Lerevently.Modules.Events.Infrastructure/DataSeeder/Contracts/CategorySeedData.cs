namespace Lerevently.Modules.Events.Infrastructure.DataSeeder.Contracts;

internal sealed record CategorySeedData(
    Guid Id,
    string Name,
    bool IsArchived
);