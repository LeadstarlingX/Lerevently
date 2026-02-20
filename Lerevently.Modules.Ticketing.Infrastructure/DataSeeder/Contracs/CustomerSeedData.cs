namespace Lerevently.Modules.Ticketing.Infrastructure.DataSeeder.Contracs;

internal sealed record CustomerSeedData(
    Guid Id,
    string Email,
    string FirstName,
    string LastName
);