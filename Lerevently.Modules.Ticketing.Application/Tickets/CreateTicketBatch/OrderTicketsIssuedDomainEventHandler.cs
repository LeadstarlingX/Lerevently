using Lerevently.Common.Application.Exceptions;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Application.Tickets.GetTicketForOrder;
using Lerevently.Modules.Ticketing.Domain.Orders;
using MediatR;

namespace Lerevently.Modules.Ticketing.Application.Tickets.CreateTicketBatch;

internal sealed class OrderTicketsIssuedDomainEventHandler(ISender sender)
    : IDomainEventHandler<OrderTicketsIssuedDomainEvent>
{
    public async Task Handle(OrderTicketsIssuedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetTicketsForOrderQuery(domainEvent.OrderId), cancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(GetTicketsForOrderQuery), result.Error);

        // Send ticket confirmation notification.
    }
}