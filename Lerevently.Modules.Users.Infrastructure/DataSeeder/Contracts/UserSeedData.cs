namespace Lerevently.Modules.Users.Infrastructure.DataSeeder.Contracts;

internal sealed record UserSeedData(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string IdentityId);