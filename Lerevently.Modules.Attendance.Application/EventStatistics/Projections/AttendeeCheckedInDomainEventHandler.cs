using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Attendance.Domain.Attendees;

namespace Lerevently.Modules.Attendance.Application.EventStatistics.Projections;

internal sealed class AttendeeCheckedInDomainEventHandler(IDbConnectionFactory dbConnectionFactory)
    : DomainEventHandler<AttendeeCheckedInDomainEvent>
{
    public override async Task Handle(
        AttendeeCheckedInDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            """
            UPDATE attendance."EventStatistics" es
            SET "AttendeesCheckedIn",
             = (
                SELECT COUNT(*)
                FROM attendance."Tickets" t
                WHERE
                    t."EventId" = es."EventId" AND
                    t."UsedAtUtc" IS NOT NULL)
            WHERE es."EventId" = @EventId
            """;

        await connection.ExecuteAsync(sql, domainEvent);
    }
}