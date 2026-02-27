using System.Data;
using Bogus;
using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Modules.Users.Infrastructure.DataSeeder.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.Modules.Users.Infrastructure.DataSeeder;

public static class DataSeeder
{
    public static async Task<List<UserSeedData>> SeedDataAsync(IApplicationBuilder app, bool forceReseed = false)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using var connection = await sqlConnectionFactory.GetDbConnectionAsync();

        return await SeedUsersAsync(connection, forceReseed);
    }

    private static async Task<List<UserSeedData>> SeedUsersAsync(IDbConnection connection, bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE users."Users" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM users."Users"
                           """;

            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0)
            {
                // Ideally we should fetch and return existing users if we want to use them for other modules
                // But for simplicity if we are not forcing reseed and data exists, we might return empty list 
                // However, Ticketing module needs them. Let's fetch them.
                const string sqlSelect = """
                                         SELECT "Id", "FirstName", "LastName", "Email" FROM users."Users"
                                         """;
                return (await connection.QueryAsync<UserSeedData>(sqlSelect)).AsList();
            }
        }

        var faker = new Faker();
        var users = new List<UserSeedData>();

        for (var i = 0; i < 10; i++)
            users.Add(new UserSeedData(
                Guid.NewGuid(),
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Internet.Email()
            ));

        const string sql = """
                           INSERT INTO users."Users"
                           ("Id", "FirstName", "LastName", "Email")
                           VALUES(@Id, @FirstName, @LastName, @Email);
                           """;

        await connection.ExecuteAsync(sql, users);

        return users;
    }
}