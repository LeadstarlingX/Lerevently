using Lerevently.Common.Application.Exceptions;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Application.Payments.RefundPaymentsForEvent;
using Lerevently.Modules.Ticketing.Domain.Events;
using MediatR;

namespace Lerevently.Modules.Ticketing.Application.Events.CancelEvent;

internal sealed class RefundPaymentsEventCanceledDomainEventHandler(ISender sender)
    : IDomainEventHandler<EventCanceledDomainEvent>
{
    public async Task Handle(EventCanceledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RefundPaymentsForEventCommand(domainEvent.EventId), cancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(RefundPaymentsForEventCommand), result.Error);
    }
}