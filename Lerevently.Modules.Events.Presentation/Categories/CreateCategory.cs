using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Events.Application.Categories.CreateCategory;
using Lerevently.Modules.Events.Presentation.ApiResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Categories;

internal class CreateCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("categories", async (CreateRequest request, ISender sender) =>
            {
                var result = await sender.Send(new CreateCategoryCommand(request.Name));

                return result.Match(Results.Ok, ApiResults.ApiResults.Problem);
            })
            .WithTags(Tags.Categories);
    }

    internal sealed class CreateRequest
    {
        public string Name { get; init; } = string.Empty;
    }
}