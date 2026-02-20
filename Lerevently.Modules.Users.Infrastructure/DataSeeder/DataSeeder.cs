﻿using Bogus;
using Dapper;
using Lerevently.Common.Application.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Lerevently.Modules.Users.Infrastructure.DataSeeder.Contracts;

namespace Lerevently.Modules.Users.Infrastructure.DataSeeder;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IApplicationBuilder app, bool forceReseed = false)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using var connection = await sqlConnectionFactory.GetDbConnectionAsync();

        await SeedUsersAsync(connection, forceReseed);
    }

    private static async Task SeedUsersAsync(IDbConnection connection, bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE users."User" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM users."User"
                           """;
            
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0)
            {
                return;
            }
        }

        var faker = new Faker();
        var users = new List<UserSeedData>();

        for (var i = 0; i < 10; i++)
        {
            users.Add(new UserSeedData(
                Guid.NewGuid(),
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Internet.Email(),
                Guid.NewGuid().ToString()
            ));
        }

        const string sql = """
                           INSERT INTO users."User"
                           ("Id", "FirstName", "LastName", "Email", "IdentityId")
                           VALUES(@Id, @FirstName, @LastName, @Email, @IdentityId);
                           """;

        await connection.ExecuteAsync(sql, users);
    }
}