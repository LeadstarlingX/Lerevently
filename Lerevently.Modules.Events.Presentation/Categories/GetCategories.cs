using Lerevently.Common.Application.Caching;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Common.Presentation.ApiResults;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Events.Application.Categories.GetCategories;
using Lerevently.Modules.Events.Application.Categories.GetCategory;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Categories;

internal class GetCategories : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("categories", async (ISender sender, ICacheService cacheService) =>
            {
                var categoryResponses = await cacheService.GetAsync<IReadOnlyCollection<CategoryResponse>>("categories");

                if (categoryResponses is not null)
                {
                    return Results.Ok(categoryResponses);
                }
                
                var result = await sender.Send(new GetCategoriesQuery());

                if (result.IsSuccess)
                {
                    await cacheService.SetAsync("categories", result.Value);
                }
                
                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .WithTags(Tags.Categories);
    }
}