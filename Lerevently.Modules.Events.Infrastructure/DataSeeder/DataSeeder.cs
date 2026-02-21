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
    public static async Task<(List<EventSeedData> Events, List<TicketTypeSeedData> TicketTypes)> SeedDataAsync(IApplicationBuilder app, bool forceReseed = false)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using var connection = await sqlConnectionFactory.GetDbConnectionAsync();

        var categoryIds = await SeedCategoriesAsync(connection, forceReseed);
        var events = await SeedEventsAsync(connection, categoryIds, forceReseed);
        var ticketTypes = await SeedTicketTypesAsync(connection, events.Select(e => e.Id).ToList(), forceReseed);
        
        return (events, ticketTypes);
    }

    private static async Task<List<Guid>> SeedCategoriesAsync(IDbConnection connection, bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE events."Categories" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM events."Categories"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0)
            {
                return (await connection.QueryAsync<Guid>("SELECT \"Id\" FROM events.\"Categories\"")).AsList();
            }
        }

        var faker = new Faker();
        var categories = new List<CategorySeedData>();
        var categoryIds = new List<Guid>();

        for (var i = 0; i < 5; i++)
        {
            var id = Guid.NewGuid();
            categoryIds.Add(id);
            categories.Add(new CategorySeedData(
                id,
                faker.Commerce.Categories(1)[0],
                false
            ));
        }

        const string sql = """
                           INSERT INTO events."Categories"
                           ("Id", "Name", "IsArchived")
                           VALUES(@Id, @Name, @IsArchived);
                           """;

        await connection.ExecuteAsync(sql, categories);
        return categoryIds;
    }

    private static async Task<List<EventSeedData>> SeedEventsAsync(IDbConnection connection, List<Guid> categoryIds, bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE events."Events" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM events."Events"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0)
            {
                const string sqlSelect = """
                                         SELECT "Id", "Title", "Description", "Location", "StartsAtUtc", "EndsAtUtc", "Status", "CategoryId" FROM events."Events"
                                         """;
                return (await connection.QueryAsync<EventSeedData>(sqlSelect)).AsList();
            }
        }

        if(!categoryIds.Any()) 
        {
            // If we have no categories, we can't create events properly. Or should we create some?
            // Assuming categories were seeded or existed.
            return [];
        }

        var faker = new Faker();
        var events = new List<EventSeedData>();
        var eventIds = new List<Guid>();

        for (var i = 0; i < 10; i++)
        {
            var id = Guid.NewGuid();
            eventIds.Add(id);
            
            var categoryId = categoryIds[faker.Random.Number(0, categoryIds.Count - 1)];

            events.Add(new EventSeedData(
                id,
                faker.Lorem.Sentence(3),
                faker.Lorem.Paragraph(),
                faker.Address.FullAddress(),
                faker.Date.Future(),
                faker.Date.Future().AddHours(2),
                1, // Published? Assuming enum or status.
                categoryId
            ));
        }

        const string sql = """
                           INSERT INTO events."Events"
                           ("Id", "Title", "Description", "Location", "StartsAtUtc", "EndsAtUtc", "Status", "CategoryId")
                           VALUES(@Id, @Title, @Description, @Location, @StartsAtUtc, @EndsAtUtc, @Status, @CategoryId);
                           """;

        await connection.ExecuteAsync(sql, events);
        return events;
    }

    private static async Task<List<TicketTypeSeedData>> SeedTicketTypesAsync(IDbConnection connection, List<Guid> eventIds, bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE events."TicketTypes" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM events."TicketTypes"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0)
            {
                const string sqlSelect = """
                                         SELECT "Id", "EventId", "Name", "Price", "Currency", "Quantity" FROM events."TicketTypes"
                                         """;
                return (await connection.QueryAsync<TicketTypeSeedData>(sqlSelect)).AsList();
            }
        }
        
        if(!eventIds.Any()) return [];

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
                           INSERT INTO events."TicketTypes"
                           ("Id", "EventId", "Name", "Price", "Currency", "Quantity")
                           VALUES(@Id, @EventId, @Name, @Price, @Currency, @Quantity);
                           """;

        await connection.ExecuteAsync(sql, ticketTypes);
        
        return ticketTypes;
    }
}
