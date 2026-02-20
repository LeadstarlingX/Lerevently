using Bogus;
using Dapper;
using Lerevently.Common.Application.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Lerevently.Modules.Ticketing.Infrastructure.DataSeeder.Contracs;

namespace Lerevently.Modules.Ticketing.Infrastructure.DataSeeder;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IApplicationBuilder app, bool forceReseed = false)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using var connection = await sqlConnectionFactory.GetDbConnectionAsync();

        await SeedCustomersAsync(connection, forceReseed);
    }

    private static async Task SeedCustomersAsync(IDbConnection connection, bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE ticketing."Customer" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM ticketing."Customer"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0)
            {
                return;
            }
        }

        var faker = new Faker();
        var customers = new List<CustomerSeedData>();

        for (var i = 0; i < 10; i++)
        {
            customers.Add(new CustomerSeedData(
                Guid.NewGuid(),
                faker.Internet.Email(),
                faker.Name.FirstName(),
                faker.Name.LastName()
            ));
        }

        const string sql = """
                           INSERT INTO ticketing."Customer"
                           ("Id", "Email", "FirstName", "LastName")
                           VALUES(@Id, @Email, @FirstName, @LastName);
                           """;

        await connection.ExecuteAsync(sql, customers);
    }
}
