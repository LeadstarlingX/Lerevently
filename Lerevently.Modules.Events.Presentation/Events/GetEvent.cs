using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Events.Application.Events.GetEvent;
using Lerevently.Modules.Events.Presentation.ApiResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Events;

internal static class GetEvent
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("events/{id}", async (Guid id, ISender sender) =>
            {
                Result<EventResponse> result = await sender.Send(new GetEventQuery(id));

                return result.Match(Results.Ok, ApiResults.ApiResults.Problem);
            })
            .WithTags(Tags.Events);
    }
}