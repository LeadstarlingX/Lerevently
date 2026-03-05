using System.Data.Common;
using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Attendance.Domain.Attendees;

namespace Lerevently.Modules.Attendance.Application.Attendees.GetAttendee;

internal sealed class GetAttendeeQueryQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetAttendeeQuery, AttendeeResponse>
{
    public async Task<Result<AttendeeResponse>> Handle(GetAttendeeQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            $"""
             SELECT
                 "Id" AS {nameof(AttendeeResponse.Id)},
                 "Email" AS {nameof(AttendeeResponse.Email)},
                 "FirstName" AS {nameof(AttendeeResponse.FirstName)},
                 "LastName" AS {nameof(AttendeeResponse.LastName)}
             FROM attendance."Attendees"
             WHERE "Id" = @CustomerId
             """;

        AttendeeResponse? customer = await connection.QuerySingleOrDefaultAsync<AttendeeResponse>(sql, request);

        if (customer is null)
        {
            return Result.Failure<AttendeeResponse>(AttendeeErrors.NotFound(request.CustomerId));
        }

        return customer;
    }
}
