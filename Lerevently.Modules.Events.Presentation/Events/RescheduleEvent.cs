using Lerevently.Common.Presentation.ApiResults;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Events.Application.Events.RescheduleEvent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Events;

internal class RescheduleEvent : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("events/{id}/reschedule", async (Guid id, RescheduleRequest request, ISender sender) =>
            {
                var result = await sender.Send(
                    new RescheduleEventCommand(id, request.StartsAtUtc, request.EndsAtUtc));

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .WithTags(Tags.Events);
    }

    internal sealed class RescheduleRequest
    {
        public DateTime StartsAtUtc { get; init; }

        public DateTime? EndsAtUtc { get; init; }
    }
}