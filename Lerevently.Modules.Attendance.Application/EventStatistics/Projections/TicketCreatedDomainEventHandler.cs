using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Attendance.Domain.Tickets;

namespace Lerevently.Modules.Attendance.Application.EventStatistics.Projections;

internal sealed class TicketCreatedDomainEventHandler(IDbConnectionFactory dbConnectionFactory)
    : DomainEventHandler<TicketCreatedDomainEvent>
{
    public override async Task Handle(
        TicketCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            """
            UPDATE attendance."EventStatistics" es
            SET "TicketsSold" = (
                SELECT COUNT(*)
                FROM attendance."Tickets" t
                WHERE t."EventId" = es."EventId")
            WHERE es."EventId" = @EventId
            """;

        await connection.ExecuteAsync(sql, domainEvent);
    }
}