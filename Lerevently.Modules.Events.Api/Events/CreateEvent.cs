using Lerevently.Modules.Events.Api.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lerevently.Modules.Events.Api.Events
{
    public static class CreateEvent
    {
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/events", async (Request request, EventsDbContext context) =>
            {
                var @event = new Event
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    Location = request.Location,
                    StartsAtUtc = request.StartsAtUtc,
                    EndsAtUtc = request.EndsAtUtc,
                    Status = EventStatus.Draft
                };

                context.Events.Add(@event);


                await context.SaveChangesAsync();


                return Results.Ok(@event.Id);
            })
                .WithTags(Tags.Events);

        }

    }

    internal sealed class Request
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public DateTime StartsAtUtc { get; set; }
        public DateTime? EndsAtUtc { get; set; }

    }
}
