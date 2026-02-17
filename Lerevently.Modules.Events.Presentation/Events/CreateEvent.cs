using Lerevently.Common.Presentation.ApiResults;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Events.Application.Events.CreateEvent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Events;

internal class CreateEvent : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("events", async (CreateRequest request, ISender sender) =>
            {
                var result = await sender.Send(new CreateEventCommand(
                    request.CategoryId,
                    request.Title,
                    request.Description,
                    request.Location,
                    request.StartsAtUtc,
                    request.EndsAtUtc));

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .WithTags(Tags.Events);
    }

    internal sealed class CreateRequest
    {
        public Guid CategoryId { get; init; }

        public string Title { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public string Location { get; init; } = string.Empty;

        public DateTime StartsAtUtc { get; init; }

        public DateTime? EndsAtUtc { get; init; }
    }
}