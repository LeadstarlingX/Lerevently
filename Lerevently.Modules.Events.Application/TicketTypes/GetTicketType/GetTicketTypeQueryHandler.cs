using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Events.Domain.TicktTypes;

namespace Lerevently.Modules.Events.Application.TicketTypes.GetTicketType;

internal sealed class GetTicketTypeQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetTicketTypeQuery, TicketTypeResponse>
{
    public async Task<Result<TicketTypeResponse>> Handle(
        GetTicketTypeQuery request,
        CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            $"""
              SELECT
                  "Id" AS {nameof(TicketTypeResponse.Id)},
                  "EventId" AS {nameof(TicketTypeResponse.EventId)},
                  "Name" AS {nameof(TicketTypeResponse.Name)},
                  "Price" AS {nameof(TicketTypeResponse.Price)},
                  "Currency" AS {nameof(TicketTypeResponse.Currency)},
                  "Quantity" AS {nameof(TicketTypeResponse.Quantity)}
              FROM events."TicketTypes"
              WHERE "Id" = @TicketTypeId
              """;

        var ticketType =
            await connection.QuerySingleOrDefaultAsync<TicketTypeResponse>(sql, request);

        if (ticketType is null)
            return Result.Failure<TicketTypeResponse>(TicketTypeErrors.NotFound(request.TicketTypeId));

        return ticketType;
    }
}