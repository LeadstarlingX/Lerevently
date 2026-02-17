using Lerevently.Common.Presentation.ApiResults;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Events.Application.Events.CancelEvent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Events;

internal class CancelEvent : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("events/{id}/cancel", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new CancelEventCommand(id));

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .WithTags(Tags.Events);
    }
}