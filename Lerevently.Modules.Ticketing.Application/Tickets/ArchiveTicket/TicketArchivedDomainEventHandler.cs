using Lerevently.Common.Application.EventBus;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Domain.Tickets;
using Lerevently.Modules.Ticketing.IntegrationEvents;

namespace Lerevently.Modules.Ticketing.Application.Tickets.ArchiveTicket;

internal sealed class TicketArchivedDomainEventHandler(IEventBus eventBus)
    : IDomainEventHandler<TicketArchivedDomainEvent>
{
    public async Task Handle(TicketArchivedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await eventBus.PublishAsync(
            new TicketArchivedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                domainEvent.TicketId,
                domainEvent.Code),
            cancellationToken);
    }
}