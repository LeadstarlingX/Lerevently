using Lerevently.Common.Application.EventBus;
using Lerevently.Common.Application.Exceptions;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Application.Orders.GetOrder;
using Lerevently.Modules.Ticketing.Domain.Orders;
using Lerevently.Modules.Ticketing.IntegrationEvents;
using MediatR;

namespace Lerevently.Modules.Ticketing.Application.Orders.CreateOrder;

internal sealed class OrderCreatedDomainEventHandler(ISender sender, IEventBus eventBus)
    : IDomainEventHandler<OrderCreatedDomainEvent>
{
    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrderQuery(notification.OrderId), cancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(GetOrderQuery), result.Error);

        await eventBus.PublishAsync(
            new OrderCreatedIntegrationEvent(
                notification.Id,
                notification.OccurredAtUtc,
                result.Value.Id,
                result.Value.CustomerId,
                result.Value.TotalPrice,
                result.Value.CreatedAtUtc,
                result.Value.OrderItems.Select(oi => new OrderItemModel
                {
                    Id = oi.OrderItemId,
                    OrderId = result.Value.Id,
                    TicketTypeId = oi.TicketTypeId,
                    Price = oi.Price,
                    UnitPrice = oi.UnitPrice,
                    Currency = oi.Currency,
                    Quantity = oi.Quantity
                }).ToList()),
            cancellationToken);
    }
}