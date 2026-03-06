using Lerevently.Common.Application.EventBus;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Domain.Events;
using Lerevently.Modules.Ticketing.IntegrationEvents;

namespace Lerevently.Modules.Ticketing.Application.Tickets.ArchiveTicketsForEvent;

internal sealed class EventTicketsArchivedDomainEventHandler(IEventBus eventBus)
     : DomainEventHandler<EventTicketsArchivedDomainEvent>
{
    public override async Task Handle(
        EventTicketsArchivedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new EventTicketsArchivedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                domainEvent.EventId),
            cancellationToken);
    }
}
