using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Attendance.Domain.Events;

namespace Lerevently.Modules.Attendance.Application.EventStatistics.GetEventStatistics;

internal sealed class GetEventStatisticsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetEventStatisticsQuery, EventStatisticsResponse>
{
    public async Task<Result<EventStatisticsResponse>> Handle(
        GetEventStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            $"""
             SELECT
                 "EventId" AS {nameof(EventStatisticsResponse.EventId)},
                 "Title" AS {nameof(EventStatisticsResponse.Title)},
                 "Description" AS {nameof(EventStatisticsResponse.Description)},
                 "Location" AS {nameof(EventStatisticsResponse.Location)},
                 "StartsAtUtc" AS {nameof(EventStatisticsResponse.StartsAtUtc)},
                 "EndsAtUtc" AS {nameof(EventStatisticsResponse.EndsAtUtc)},
                 "TicketsSold" AS {nameof(EventStatisticsResponse.TicketsSold)},
                 "AttendeesCheckedIn" AS {nameof(EventStatisticsResponse.AttendeesCheckedIn)},
                 "DuplicateCheckInTickets" AS {nameof(EventStatisticsResponse.DuplicateCheckInTickets)},
                 "InvalidCheckInTickets" AS {nameof(EventStatisticsResponse.InvalidCheckInTickets)}
             FROM attendance."EventStatistics"
             WHERE "EventId" = @EventId
             """;

        var eventStatistics =
            await connection.QuerySingleOrDefaultAsync<EventStatisticsResponse>(sql, request);

        if (eventStatistics is null)
            return Result.Failure<EventStatisticsResponse>(EventErrors.NotFound(request.EventId));

        return eventStatistics;
    }
}