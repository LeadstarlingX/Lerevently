using System.Data.Common;
using Dapper;
using Lerevently.Modules.Events.Application.Abstractions.Data;
using MediatR;

namespace Lerevently.Modules.Events.Application.Events.GetEvent
{
    public sealed class GetEventQueryHandler(IDbConnectionFactory dbConnectionFactory) : IRequestHandler<GetEventQuery, EventResponse?>
    {

        public async Task<EventResponse?> Handle(GetEventQuery request, CancellationToken cancellationToken)
        {
            await using DbConnection dbConnection = await dbConnectionFactory.GetDbConnectionAsync();
            
            const string sql =
                $"""
                 SELECT
                     id AS {nameof(EventResponse.Id)},
                     title AS {nameof(EventResponse.Title)},
                     description AS {nameof(EventResponse.Description)},
                     location AS {nameof(EventResponse.Location)},
                     starts_at_utc AS {nameof(EventResponse.StartsAtUtc)},
                     ends_at_utc AS {nameof(EventResponse.EndsAtUtc)}
                 FROM events.events
                 WHERE id = @EventId
                 """;

            EventResponse? @event = await dbConnection.QuerySingleOrDefaultAsync<EventResponse>(sql, request);
            
            return @event;
        }
    }
    
}
