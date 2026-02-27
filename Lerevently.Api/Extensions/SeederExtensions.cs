using Lerevently.Modules.Ticketing.Infrastructure.DataSeeder.Contracts;
using Lerevently.Modules.Users.Infrastructure.DataSeeder;

namespace Lerevently.Api.Extensions;

public static class SeederExtensions
{
    public static async Task SeedDataAsync(this IApplicationBuilder app, bool forceReseed = false)
    {
        var users = await DataSeeder.SeedDataAsync(app, forceReseed);
        var (events, ticketTypes) =
            await Modules.Events.Infrastructure.DataSeeder.DataSeeder.SeedDataAsync(app, forceReseed);

        var ticketingUsers = users.Select(u => new UserSeedData(
            u.Id,
            u.FirstName,
            u.LastName,
            u.Email
        )).ToList();

        var ticketingEvents = events.Select(e => new EventSeedData(
            e.Id,
            e.Title,
            e.Description,
            e.Location,
            e.StartsAtUtc,
            e.EndsAtUtc,
            e.Status,
            e.CategoryId
        )).ToList();

        var ticketingTicketTypes = ticketTypes.Select(tt => new TicketTypeSeedData(
            tt.Id,
            tt.EventId,
            tt.Name,
            tt.Price,
            tt.Currency,
            tt.Quantity
        )).ToList();

        await Modules.Ticketing.Infrastructure.DataSeeder.DataSeeder.SeedDataAsync(
            app,
            ticketingUsers,
            ticketingEvents,
            ticketingTicketTypes,
            forceReseed);
    }
}