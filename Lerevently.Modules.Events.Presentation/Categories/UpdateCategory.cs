using Lerevently.Common.Presentation.ApiResults;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Events.Application.Categories.UpdateCategory;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Categories;

internal sealed class UpdateCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("categories/{id}", async (Guid id, Request request, ISender sender) =>
            {
                var result = await sender.Send(new UpdateCategoryCommand(id, request.Name));

                return result.Match(() => Results.Ok(), ApiResults.Problem);
            })
            .RequireAuthorization(Permissions.ModifyCategories)
            .WithTags(Tags.Categories);
    }

    private sealed class Request
    {
        public string Name { get; init; } = string.Empty;
    }
}