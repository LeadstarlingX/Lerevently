using Lerevently.Common.Application.Authorization;
using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Users.Application.Users.GetUserPermissions;

public sealed record GetUserPermissionsQuery(string IdentityId) : IQuery<PermissionsResponse>;