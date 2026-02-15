using Lerevently.Modules.Events.Application.Events;
using Lerevently.Modules.Events.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Events
{
    internal static class CreateEvent
    {
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/events", async (Request request, ISender sender) =>
                {
                    var command = new CreateEventCommand(
                        request.Title,
                        request.Description,
                        request.Location,
                        request.StartsAtUtc,
                        request.EndsAtUtc
                    );
                
                    var eventId = await sender.Send(command);
                    
                    
                    return Results.Ok(eventId);
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
