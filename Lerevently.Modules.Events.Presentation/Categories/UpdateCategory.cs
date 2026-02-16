using Lerevently.Modules.Events.Application.Categories.UpdateCategory;
using Lerevently.Modules.Events.Presentation.ApiResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Categories;

internal static class UpdateCategory
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("categories/{id}", async (Guid id, UpdateRequest request, ISender sender) =>
            {
                var result = await sender.Send(new UpdateCategoryCommand(id, request.Name));

                return result.Match(() => Results.Ok(), ApiResults.ApiResults.Problem);
            })
            .WithTags(Tags.Categories);
    }

    internal sealed class UpdateRequest
    {
        public string Name { get; init; } = string.Empty;
    }
}