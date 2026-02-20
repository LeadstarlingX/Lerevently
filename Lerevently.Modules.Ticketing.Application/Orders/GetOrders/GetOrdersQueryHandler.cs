using System.Data.Common;
using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Ticketing.Application.Orders.GetOrders;

internal sealed class GetOrdersQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetOrdersQuery, IReadOnlyCollection<OrderResponse>>
{
    public async Task<Result<IReadOnlyCollection<OrderResponse>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            $"""
              SELECT
                  "Id" AS {nameof(OrderResponse.Id)},
                  "CustomerId" AS {nameof(OrderResponse.CustomerId)},
                  "Status" AS {nameof(OrderResponse.Status)},
                  "TotalPrice" AS {nameof(OrderResponse.TotalPrice)},
                  "CreatedAtUtc" AS {nameof(OrderResponse.CreatedAtUtc)}
              FROM ticketing."Orders"
              WHERE "CustomerId" = @CustomerId
              """;

        List<OrderResponse> orders = (await connection.QueryAsync<OrderResponse>(sql, request)).AsList();

        return orders;
    }
}
