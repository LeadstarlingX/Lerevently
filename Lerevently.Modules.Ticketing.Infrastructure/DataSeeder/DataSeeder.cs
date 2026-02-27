using System.Data;
using Bogus;
using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Modules.Ticketing.Infrastructure.DataSeeder.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.Modules.Ticketing.Infrastructure.DataSeeder;

internal sealed record TicketTypeProjection(Guid Id, Guid EventId);

public static class DataSeeder
{
    public static async Task SeedDataAsync(
        IApplicationBuilder app,
        List<UserSeedData> users,
        List<EventSeedData> events,
        List<TicketTypeSeedData> ticketTypes,
        bool forceReseed = false)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using var connection = await sqlConnectionFactory.GetDbConnectionAsync();

        await SeedCustomersAsync(connection, users, forceReseed);
        await SeedEventsAsync(connection, events, forceReseed);
        await SeedTicketTypesAsync(connection, ticketTypes, forceReseed);

        var orders = await SeedOrdersAsync(connection, users, ticketTypes, forceReseed);
        await SeedPaymentsAsync(connection, orders, forceReseed);
        await SeedTicketsAsync(connection, orders, forceReseed);
    }

    private static async Task SeedCustomersAsync(IDbConnection connection, List<UserSeedData> users, bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE ticketing."Customers" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM ticketing."Customers"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0) return;
        }

        if (!users.Any()) return;

        var customers = users.Select(u => new CustomerSeedData(u.Id, u.Email, u.FirstName, u.LastName)).ToList();

        const string sql = """
                           INSERT INTO ticketing."Customers"
                           ("Id", "Email", "FirstName", "LastName")
                           VALUES(@Id, @Email, @FirstName, @LastName);
                           """;

        await connection.ExecuteAsync(sql, customers);
    }

    private static async Task SeedEventsAsync(IDbConnection connection, List<EventSeedData> events, bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE ticketing."Events" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM ticketing."Events"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0) return;
        }

        if (!events.Any()) return;

        var eventProjections = events.Select(e => new EventProjectionSeedData(
            e.Id,
            e.Title,
            e.Description,
            e.Location,
            e.StartsAtUtc,
            e.EndsAtUtc,
            false // Canceled
        )).ToList();

        const string sql = """
                           INSERT INTO ticketing."Events"
                           ("Id", "Title", "Description", "Location", "StartsAtUtc", "EndsAtUtc", "Canceled")
                           VALUES(@Id, @Title, @Description, @Location, @StartsAtUtc, @EndsAtUtc, @Canceled);
                           """;

        await connection.ExecuteAsync(sql, eventProjections);
    }

    private static async Task SeedTicketTypesAsync(IDbConnection connection, List<TicketTypeSeedData> ticketTypes,
        bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE ticketing."TicketTypes" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM ticketing."TicketTypes"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0) return;
        }

        if (!ticketTypes.Any()) return;

        var ticketTypeProjections = ticketTypes.Select(tt => new TicketTypeProjectionSeedData(
            tt.Id,
            tt.EventId,
            tt.Name,
            tt.Price,
            tt.Currency,
            tt.Quantity,
            tt.Quantity // AvailableQuantity starts same as Quantity
        )).ToList();

        const string sql = """
                           INSERT INTO ticketing."TicketTypes"
                           ("Id", "EventId", "Name", "Price", "Currency", "Quantity", "AvailableQuantity")
                           VALUES(@Id, @EventId, @Name, @Price, @Currency, @Quantity, @AvailableQuantity);
                           """;

        await connection.ExecuteAsync(sql, ticketTypeProjections);
    }

    private static async Task<List<(OrderSeedData Order, List<OrderItemSeedData> Items)>> SeedOrdersAsync(
        IDbConnection connection,
        List<UserSeedData> users,
        List<TicketTypeSeedData> ticketTypes,
        bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE ticketing."Orders" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM ticketing."Orders"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0) return [];
        }

        if (!users.Any() || !ticketTypes.Any()) return [];

        var faker = new Faker();
        var seededOrders = new List<(OrderSeedData, List<OrderItemSeedData>)>();

        // Create some orders for random users
        for (var i = 0; i < 20; i++)
        {
            var user = faker.PickRandom(users);
            var orderId = Guid.NewGuid();
            var creationDate = faker.Date.Past();

            // Pick 1-3 ticket types to buy
            var numItems = faker.Random.Number(1, 3);
            var items = new List<OrderItemSeedData>();
            decimal totalPrice = 0;
            var currency = "USD";

            for (var j = 0; j < numItems; j++)
            {
                var tt = faker.PickRandom(ticketTypes);
                currency = tt.Currency; // Assuming single currency for simplicity
                var qty = faker.Random.Number(1, 4);
                var price = tt.Price * qty;

                items.Add(new OrderItemSeedData(
                    Guid.NewGuid(),
                    orderId,
                    tt.Id,
                    qty,
                    tt.Price,
                    price,
                    tt.Currency
                ));
                totalPrice += price;
            }

            var order = new OrderSeedData(
                orderId,
                user.Id,
                1, // Status: Paid/Completed? Assuming 1 is valid
                totalPrice,
                currency,
                true, // TicketsIssued
                creationDate
            );

            seededOrders.Add((order, items));
        }

        const string sqlOrder = """
                                INSERT INTO ticketing."Orders"
                                ("Id", "CustomerId", "Status", "TotalPrice", "Currency", "TicketsIssued", "CreatedAtUtc")
                                VALUES(@Id, @CustomerId, @Status, @TotalPrice, @Currency, @TicketsIssued, @CreatedAtUtc);
                                """;

        const string sqlOrderItem = """
                                    INSERT INTO ticketing."OrderItems"
                                    ("Id", "OrderId", "TicketTypeId", "Quantity", "UnitPrice", "Price", "Currency")
                                    VALUES(@Id, @OrderId, @TicketTypeId, @Quantity, @UnitPrice, @Price, @Currency);
                                    """;

        foreach (var (order, items) in seededOrders)
        {
            await connection.ExecuteAsync(sqlOrder, order);
            await connection.ExecuteAsync(sqlOrderItem, items);
        }

        return seededOrders;
    }

    private static async Task SeedPaymentsAsync(
        IDbConnection connection,
        List<(OrderSeedData Order, List<OrderItemSeedData> Items)> orders,
        bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE ticketing."Payments" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM ticketing."Payments"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0) return;
        }

        if (!orders.Any()) return;

        var payments = new List<PaymentSeedData>();

        foreach (var (order, _) in orders)
            payments.Add(new PaymentSeedData(
                Guid.NewGuid(),
                order.Id,
                Guid.NewGuid(),
                order.TotalPrice,
                order.Currency,
                null,
                order.CreatedAtUtc.AddSeconds(10), // Paid shortly after
                null
            ));

        const string sql = """
                           INSERT INTO ticketing."Payments"
                           ("Id", "OrderId", "TransactionId", "Amount", "Currency", "AmountRefunded", "CreatedAtUtc", "RefundedAtUtc")
                           VALUES(@Id, @OrderId, @TransactionId, @Amount, @Currency, @AmountRefunded, @CreatedAtUtc, @RefundedAtUtc);
                           """;

        await connection.ExecuteAsync(sql, payments);
    }

    private static async Task SeedTicketsAsync(
        IDbConnection connection,
        List<(OrderSeedData Order, List<OrderItemSeedData> Items)> orders,
        bool forceReseed)
    {
        if (forceReseed)
        {
            const string sqlTruncate = """
                                       TRUNCATE TABLE ticketing."Tickets" RESTART IDENTITY CASCADE;
                                       """;
            await connection.ExecuteAsync(sqlTruncate);
        }
        else
        {
            var sqlCount = """
                           SELECT COUNT(*) FROM ticketing."Tickets"
                           """;
            var count = await connection.ExecuteScalarAsync<int>(sqlCount);
            if (count > 0) return;
        }

        if (!orders.Any()) return;

        // We need to fetch TicketTypes to know the EventId for each TicketTypeId
        // Ideally we should have passed it, but we can query it or map it from previous step if available.
        // Actually, we passed 'ticketTypes' list to 'SeedOrdersAsync' but not here.
        // Let's just query from DB for simplicity as we are inside the module seeding.
        var ticketTypesMap = (await connection.QueryAsync<TicketTypeProjection>(
                """SELECT "Id", "EventId" FROM ticketing."TicketTypes" """))
            .ToDictionary(k => k.Id, v => v.EventId);

        var faker = new Faker();
        var tickets = new List<TicketSeedData>();

        foreach (var (order, items) in orders)
        foreach (var item in items)
        {
            if (!ticketTypesMap.ContainsKey(item.TicketTypeId)) continue;
            var eventId = ticketTypesMap[item.TicketTypeId];

            for (var k = 0; k < item.Quantity; k++)
                tickets.Add(new TicketSeedData(
                    Guid.NewGuid(),
                    order.CustomerId,
                    order.Id,
                    eventId,
                    item.TicketTypeId,
                    faker.Random.AlphaNumeric(8).ToUpper(),
                    order.CreatedAtUtc.AddSeconds(5),
                    false
                ));
        }

        const string sql = """
                           INSERT INTO ticketing."Tickets"
                           ("Id", "CustomerId", "OrderId", "EventId", "TicketTypeId", "Code", "CreatedAtUtc", "Archived")
                           VALUES(@Id, @CustomerId, @OrderId, @EventId, @TicketTypeId, @Code, @CreatedAtUtc, @Archived);
                           """;

        await connection.ExecuteAsync(sql, tickets);
    }
}