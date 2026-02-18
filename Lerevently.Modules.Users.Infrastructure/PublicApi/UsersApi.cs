using Evently.Modules.Users.PublicApi;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Users.Application.Users.GetUser;
using MediatR;
using UserResponse = Evently.Modules.Users.PublicApi.UserResponse;

namespace Lerevently.Modules.Users.Infrastructure.PublicApi;

internal sealed class UsersApi(ISender sender) : IUsersApi
{
    public async Task<UserResponse?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        Result<Application.Users.GetUser.UserResponse> result =
            await sender.Send(new GetUserQuery(userId), cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        return new UserResponse(
            result.Value.Id,
            result.Value.Email,
            result.Value.FirstName,
            result.Value.LastName);
    }
}
