using Lerevently.Common.Application.Authorization;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Users.Application.Users.GetUserPermissions;
using MediatR;

namespace Lerevently.Modules.Users.Infrastructure.Authorization;

internal sealed class PermissionService(ISender sender) : IPermissionService
{
    public async Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId)
    {
        return await sender.Send(new GetUserPermissionsQuery(identityId));
    }
}
