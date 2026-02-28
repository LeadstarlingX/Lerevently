using Lerevently.Common.Application.Exceptions;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Application.Tickets.CreateTicketBatch;
using Lerevently.Modules.Ticketing.Domain.Orders;
using MediatR;

namespace Lerevently.Modules.Ticketing.Application.Orders.CreateOrder;

internal sealed class CreateTicketsDomainEventHandler(ISender sender)
    : DomainEventHandler<OrderCreatedDomainEvent>
{
    public override async Task Handle(OrderCreatedDomainEvent notification,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new CreateTicketBatchCommand(notification.OrderId), cancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(CreateTicketBatchCommand), result.Error);
    }
}