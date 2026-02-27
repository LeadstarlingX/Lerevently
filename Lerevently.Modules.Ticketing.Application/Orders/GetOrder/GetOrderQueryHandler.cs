using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Domain.Orders;

namespace Lerevently.Modules.Ticketing.Application.Orders.GetOrder;

internal sealed class GetOrderQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetOrderQuery, OrderResponse>
{
    public async Task<Result<OrderResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.GetDbConnectionAsync();

        const string sql =
            $"""
             SELECT
                 o."Id" AS {nameof(OrderResponse.Id)},
                 o."CustomerId" AS {nameof(OrderResponse.CustomerId)},
                 o."Status" AS {nameof(OrderResponse.Status)},
                 o."TotalPrice" AS {nameof(OrderResponse.TotalPrice)},
                 o."CreatedAtUtc" AS {nameof(OrderResponse.CreatedAtUtc)},
                 oi."Id" AS {nameof(OrderItemResponse.OrderItemId)},
                 oi."OrderId" AS {nameof(OrderItemResponse.OrderId)},
                 oi."TicketTypeId" AS {nameof(OrderItemResponse.TicketTypeId)},
                 oi."Quantity" AS {nameof(OrderItemResponse.Quantity)},
                 oi."UnitPrice" AS {nameof(OrderItemResponse.UnitPrice)},
                 oi."Price" AS {nameof(OrderItemResponse.Price)},
                 oi."Currency" AS {nameof(OrderItemResponse.Currency)}
             FROM ticketing."Orders" o
             JOIN ticketing."OrderItems" oi ON oi."OrderId" = o."Id"
             WHERE o."Id" = @OrderId
             """;

        Dictionary<Guid, OrderResponse> ordersDictionary = [];
        await connection.QueryAsync<OrderResponse, OrderItemResponse, OrderResponse>(
            sql,
            (order, orderItem) =>
            {
                if (ordersDictionary.TryGetValue(order.Id, out var existingEvent))
                    order = existingEvent;
                else
                    ordersDictionary.Add(order.Id, order);

                order.OrderItems.Add(orderItem);

                return order;
            },
            request,
            splitOn: nameof(OrderItemResponse.OrderItemId));

        if (!ordersDictionary.TryGetValue(request.OrderId, out var orderResponse))
            return Result.Failure<OrderResponse>(OrderErrors.NotFound(request.OrderId));

        return orderResponse;
    }
}