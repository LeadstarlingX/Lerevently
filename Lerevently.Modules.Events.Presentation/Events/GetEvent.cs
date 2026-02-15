using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Events
{
    internal sealed class GetEvent
    {
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("events/{id}", async (Guid id, ISender sender) =>
            {
                var @event = await sender.Send(new Application.Events.GetEventQuery(id));

                return @event is null ? Results.NotFound() : Results.Ok(@event);

            })
              .WithTags(Tags.Events);
        }

    }

}
