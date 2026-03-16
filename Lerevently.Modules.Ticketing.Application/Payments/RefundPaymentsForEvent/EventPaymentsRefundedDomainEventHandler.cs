using Lerevently.Common.Application.EventBus;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Domain.Events;
using Lerevently.Modules.Ticketing.IntegrationEvents;

namespace Lerevently.Modules.Ticketing.Application.Payments.RefundPaymentsForEvent;

internal sealed class EventPaymentsRefundedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<EventPaymentsRefundedDomainEvent>
{
    public override async Task Handle(
        EventPaymentsRefundedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new EventPaymentsRefundedIntegrationEvent(
                domainEvent.EventId,
                domainEvent.OccurredAtUtc,
                domainEvent.EventId),
            cancellationToken);
    }
}