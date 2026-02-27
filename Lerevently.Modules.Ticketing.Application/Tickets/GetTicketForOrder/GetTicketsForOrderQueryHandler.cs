using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Application.Tickets.GetTicket;

namespace Lerevently.Modules.Ticketing.Application.Tickets.GetTicketForOrder;

internal sealed class GetTicketsForOrderQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetTicketsForOrderQuery, IReadOnlyCollection<TicketResponse>>
{
    public async Task<Result<IReadOnlyCollection<TicketResponse>>> Handle(
        GetTicketsForOrderQuery request,
        CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.GetDbConnectionAsync();

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
             WHERE "OrderId" = @OrderId
             """;

        var tickets = (await connection.QueryAsync<TicketResponse>(sql, request)).AsList();

        return tickets;
    }
}