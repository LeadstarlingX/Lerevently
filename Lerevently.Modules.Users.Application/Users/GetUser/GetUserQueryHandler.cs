using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Users.Domain.Users;

namespace Lerevently.Modules.Users.Application.Users.GetUser;

internal sealed class GetUserQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            $"""
             SELECT
                 "Id" AS {nameof(UserResponse.Id)},
                 "Email" AS {nameof(UserResponse.Email)},
                 "FirstName" AS {nameof(UserResponse.FirstName)},
                 "LastName" AS {nameof(UserResponse.LastName)}
             FROM users."Users"
             WHERE "Id" = @UserId
             """;

        var user = await connection.QuerySingleOrDefaultAsync<UserResponse>(sql, request);

        Console.WriteLine(user.Id);

        if (user is null)
        {
            Console.WriteLine("Executed");
            return Result.Failure<UserResponse>(UserErrors.NotFound(request.UserId));
        }

        return user;
    }
}