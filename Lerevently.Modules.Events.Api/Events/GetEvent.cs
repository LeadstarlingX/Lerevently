using Lerevently.Modules.Events.Api.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lerevently.Modules.Events.Api.Events
{
    public sealed class GetEvent
    {
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("events/{id}", async (Guid id, EventsDbContext context) =>
            {
                EventResponse? @event = await context.Events
                .Where(e => e.Id == id)
                .Select(s => new EventResponse
                (
                    s.Id,
                    s.Title,
                    s.Description,
                    s.Location,
                    s.StartsAtUtc,
                    s.EndsAtUtc
                ))
                .SingleOrDefaultAsync();

                return @event is null ? Results.NotFound() : Results.Ok(@event);

            })
              .WithTags(Tags.Events);
        }

    }

}
