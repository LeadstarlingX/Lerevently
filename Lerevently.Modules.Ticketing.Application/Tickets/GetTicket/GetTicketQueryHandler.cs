using System.Data.Common;
using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Domain.Tickets;

namespace Lerevently.Modules.Ticketing.Application.Tickets.GetTicket;

internal sealed class GetTicketQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetTicketQuery, TicketResponse>
{
    public async Task<Result<TicketResponse>> Handle(GetTicketQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            $"""
            SELECT
                "Id" AS {nameof(TicketResponse.Id)},
                "CustomerId" AS {nameof(TicketResponse.CustomerId)},
                "OrderId" AS {nameof(TicketResponse.OrderId)},
                "EventId" AS {nameof(TicketResponse.EventId)},
                "TicketTypeId" AS {nameof(TicketResponse.TicketTypeId)},
                "Code" AS {nameof(TicketResponse.Code)},
                "CreatedAtUtc" AS {nameof(TicketResponse.CreatedAtUtc)}
            FROM ticketing."Tickets"
            WHERE "Id" = @TicketId
            """;

        TicketResponse? ticket = await connection.QuerySingleOrDefaultAsync<TicketResponse>(sql, request);

        if (ticket is null)
        {
            return Result.Failure<TicketResponse>(TicketErrors.NotFound(request.TicketId));
        }

        return ticket;
    }
}
