using Dapper;
using Lerevently.Common.Application.Authorization;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Users.Domain.Users;

namespace Lerevently.Modules.Users.Application.Users.GetUserPermissions;

internal sealed class GetUserPermissionsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetUserPermissionsQuery, PermissionsResponse>
{
    public async Task<Result<PermissionsResponse>> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            """
            SELECT DISTINCT
                u."Id" AS UserId,
                rp."PermissionCode" AS Permission
            FROM users."Users" u
            JOIN users.user_roles ur ON ur."UserId" = u."Id"
            JOIN users.role_permissions rp ON rp."RoleName" = ur.role_name
            WHERE u."IdentityId" = @IdentityId
            """;

        var permissions = (await connection.QueryAsync<UserPermission>(sql, request)).AsList();

        if (!permissions.Any()) return Result.Failure<PermissionsResponse>(UserErrors.NotFound(request.IdentityId));

        return new PermissionsResponse(permissions[0].UserId, permissions.Select(p => p.Permission).ToHashSet());
    }

    internal sealed class UserPermission
    {
        internal Guid UserId { get; init; }

        internal string Permission { get; init; } = string.Empty;
    }
}