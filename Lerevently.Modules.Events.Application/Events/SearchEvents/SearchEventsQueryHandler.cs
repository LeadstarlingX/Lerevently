using System.Data.Common;
using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Events.Application.Events.GetEvents;
using Lerevently.Modules.Events.Domain.Events;

namespace Lerevently.Modules.Events.Application.Events.SearchEvents;

internal sealed class SearchEventsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<SearchEventsQuery, SearchEventsResponse>
{
    public async Task<Result<SearchEventsResponse>> Handle(
        SearchEventsQuery request,
        CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.GetDbConnectionAsync();

        var parameters = new SearchEventsParameters(
            (int)EventStatus.Published,
            request.CategoryId,
            request.StartDate?.Date,
            request.EndDate?.Date,
            request.PageSize,
            (request.Page - 1) * request.PageSize);

        var events = await GetEventsAsync(connection, parameters);

        var totalCount = await CountEventsAsync(connection, parameters);

        return new SearchEventsResponse(request.Page, request.PageSize, totalCount, events);
    }

    private static async Task<IReadOnlyCollection<EventResponse>> GetEventsAsync(
        DbConnection connection,
        SearchEventsParameters parameters)
    {
        const string sql =
            $"""
             SELECT
                 "Id" AS {nameof(EventResponse.Id)},
                 "CategoryId" AS {nameof(EventResponse.CategoryId)},
                 "Title" AS {nameof(EventResponse.Title)},
                 "Description" AS {nameof(EventResponse.Description)},
                 "Location" AS {nameof(EventResponse.Location)},
                 "StartsAtUtc" AS {nameof(EventResponse.StartsAtUtc)},
                 "EndsAtUtc" AS {nameof(EventResponse.EndsAtUtc)}
             FROM events."Events"
             WHERE
                "Status" = @Status AND
                (@CategoryId IS NULL OR "CategoryId" = @CategoryId) AND
                (@StartDate::timestamp IS NULL OR "StartsAtUtc" >= @StartDate::timestamp) AND
                (@EndDate::timestamp IS NULL OR "EndsAtUtc" >= @EndDate::timestamp)
             ORDER BY "StartsAtUtc"
             OFFSET @Skip
             LIMIT @Take
             """;

        var events = (await connection.QueryAsync<EventResponse>(sql, parameters)).AsList();

        return events;
    }

    private static async Task<int> CountEventsAsync(DbConnection connection, SearchEventsParameters parameters)
    {
        const string sql =
            """
            SELECT COUNT(*)
            FROM events.events
            WHERE
               status = @Status AND
               (@CategoryId IS NULL OR category_id = @CategoryId) AND
               (@StartDate::timestamp IS NULL OR starts_at_utc >= @StartDate::timestamp) AND
               (@EndDate::timestamp IS NULL OR ends_at_utc >= @EndDate::timestamp)
            """;

        var totalCount = await connection.ExecuteScalarAsync<int>(sql, parameters);

        return totalCount;
    }

    private sealed record SearchEventsParameters(
        int Status,
        Guid? CategoryId,
        DateTime? StartDate,
        DateTime? EndDate,
        int Take,
        int Skip);
}