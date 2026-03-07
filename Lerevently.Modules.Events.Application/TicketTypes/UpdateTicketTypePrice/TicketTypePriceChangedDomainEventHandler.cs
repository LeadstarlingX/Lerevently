using Lerevently.Common.Application.EventBus;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Events.Domain.TicketTypes;
using Lerevently.Modules.Events.IntegrationEvents;

namespace Lerevently.Modules.Events.Application.TicketTypes.UpdateTicketTypePrice;

internal sealed class TicketTypePriceChangedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<TicketTypePriceChangedDomainEvent>
{
    public override async Task Handle(TicketTypePriceChangedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new TicketTypePriceChangedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                domainEvent.TicketTypeId,
                domainEvent.Price),
            cancellationToken);
    }
}