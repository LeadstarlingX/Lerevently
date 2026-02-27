namespace Lerevently.Modules.Users.Infrastructure.DataSeeder.Contracts;

public sealed record UserSeedData(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);