using Dapper;
using Lerevently.Common.Application.Data;
using MediatR;

namespace Lerevently.Modules.Events.Application.Events.GetEvent;

public sealed class GetEventQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IRequestHandler<GetEventQuery, EventResponse?>
{
    public async Task<EventResponse?> Handle(GetEventQuery request, CancellationToken cancellationToken)
    {
        await using var dbConnection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            $"""
              SELECT
                  "Id" AS {nameof(EventResponse.Id)},
                  "Title" AS {nameof(EventResponse.Title)},
                  "Description" AS {nameof(EventResponse.Description)},
                  "Location" AS {nameof(EventResponse.Location)},
                  "StartsAtUtc" AS {nameof(EventResponse.StartsAtUtc)},
                  "EndsAtUtc" AS {nameof(EventResponse.EndsAtUtc)}
              FROM events."Events"
              WHERE "Id" = @EventId
              """;

        var @event = await dbConnection.QuerySingleOrDefaultAsync<EventResponse>(sql, request);

        return @event;
    }
}