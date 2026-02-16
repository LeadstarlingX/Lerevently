using Lerevently.Modules.Events.Application.Categories.GetCategories;
using Lerevently.Modules.Events.Presentation.ApiResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Categories;

internal static class GetCategories
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("categories", async (ISender sender) =>
            {
                var result = await sender.Send(new GetCategoriesQuery());

                return result.Match(Results.Ok, ApiResults.ApiResults.Problem);
            })
            .WithTags(Tags.Categories);
    }
}