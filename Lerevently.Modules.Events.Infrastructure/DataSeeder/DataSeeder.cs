using Bogus;
using Dapper;
using Lerevently.Common.Application.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Lerevently.Modules.Events.Infrastructure.DataSeeder.Contracts;

namespace Lerevently.Modules.Events.Infrastructure.DataSeeder;



public static class DataSeeder
{
    public static async Task SeedDataAsync(IApplicationBuilder app, bool forceReseed = false)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using var connection = await sqlConnectionFactory.GetDbConnectionAsync();

        await SeedCategoriesAsync(connection, forceReseed);
        var eventIds = await SeedEventsAsync(connection, forceReseed);
        await SeedTicketTypesAsync(connection, eventIds, forceReseed);
    }

    private static async Task SeedCategoriesAsync(IDbConnection connection, bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE events."Category" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM events."Category"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0)
            {
                return;
            }
        }

        var faker = new Faker();
        var categories = new List<CategorySeedData>();

        for (var i = 0; i < 5; i++)
        {
            categories.Add(new CategorySeedData(
                Guid.NewGuid(),
                faker.Commerce.Categories(1)[0],
                false
            ));
        }

        const string sql = """
                           INSERT INTO events."Category"
                           ("Id", "Name", "IsArchived")
                           VALUES(@Id, @Name, @IsArchived);
                           """;

        await connection.ExecuteAsync(sql, categories);
    }

    private static async Task<List<Guid>> SeedEventsAsync(IDbConnection connection, bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE events."Event" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM events."Event"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0)
            {
                return (await connection.QueryAsync<Guid>("SELECT \"Id\" FROM events.\"Event\"")).AsList();
            }
        }

        var faker = new Faker();
        var events = new List<EventSeedData>();
        var eventIds = new List<Guid>();

        for (var i = 0; i < 10; i++)
        {
            var id = Guid.NewGuid();
            eventIds.Add(id);
            events.Add(new EventSeedData(
                id,
                faker.Lorem.Sentence(3),
                faker.Lorem.Paragraph(),
                faker.Address.FullAddress(),
                faker.Date.Future(),
                faker.Date.Future().AddHours(2),
                1 // Published? Assuming enum or status.
            ));
        }

        const string sql = """
                           INSERT INTO events."Event"
                           ("Id", "Title", "Description", "Location", "StartsAtUtc", "EndsAtUtc", "Status")
                           VALUES(@Id, @Title, @Description, @Location, @StartsAtUtc, @EndsAtUtc, @Status);
                           """;

        await connection.ExecuteAsync(sql, events);
        return eventIds;
    }

    private static async Task SeedTicketTypesAsync(IDbConnection connection, List<Guid> eventIds, bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE events."TicketType" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM events."TicketType"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0)
            {
                return;
            }
        }

        if(!eventIds.Any()) return;

        var faker = new Faker();
        var ticketTypes = new List<TicketTypeSeedData>();

        foreach (var eventId in eventIds)
        {
             // Add a few ticket types per event
             ticketTypes.Add(new TicketTypeSeedData(
                 Guid.NewGuid(),
                 eventId,
                 "General Admission",
                 decimal.Parse(faker.Commerce.Price(10, 100)),
                 "USD",
                 100
             ));
             
             ticketTypes.Add(new TicketTypeSeedData(
                 Guid.NewGuid(),
                 eventId,
                 "VIP",
                 decimal.Parse(faker.Commerce.Price(100, 500)),
                 "USD",
                 20
             ));
        }

        const string sql = """
                           INSERT INTO events."TicketType"
                           ("Id", "EventId", "Name", "Price", "Currency", "Quantity")
                           VALUES(@Id, @EventId, @Name, @Price, @Currency, @Quantity);
                           """;

        await connection.ExecuteAsync(sql, ticketTypes);
    }
}
