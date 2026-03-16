using System.Security.Claims;
using Lerevently.Common.Infrastructure.Authentication;
using Lerevently.Common.Presentation.ApiResults;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Users.Application.Users.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Users.Presentation.Users;

internal sealed class UpdateUserProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/profile", async (Request request, ClaimsPrincipal claims, ISender sender) =>
            {
                var result = await sender.Send(new UpdateUserCommand(
                    claims.GetUserId(),
                    request.FirstName,
                    request.LastName));

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .RequireAuthorization(Permissions.ModifyUser)
            .WithTags(Tags.Users);
    }

    private sealed class Request
    {
        public string FirstName { get; } = string.Empty;

        public string LastName { get; } = string.Empty;
    }
}