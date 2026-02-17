using Lerevently.Common.Presentation.ApiResults;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Events.Application.TicketTypes.GetTicketTypes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.TicketTypes;

internal class GetTicketTypes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ticket-types", async (Guid eventId, ISender sender) =>
            {
                var result = await sender.Send(
                    new GetTicketTypesQuery(eventId));

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .WithTags(Tags.TicketTypes);
    }
}