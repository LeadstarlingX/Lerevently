using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Common.Application.Authorization;

public interface IPermissionService
{
    Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId);
}
