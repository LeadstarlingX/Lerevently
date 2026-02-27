using Lerevently.Common.Application.EventBus;
using Lerevently.Common.Application.Exceptions;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Application.Tickets.GetTicket;
using Lerevently.Modules.Ticketing.Domain.Tickets;
using Lerevently.Modules.Ticketing.IntegrationEvents;
using MediatR;

namespace Lerevently.Modules.Ticketing.Application.Tickets.CreateTicketBatch;

internal sealed class TicketCreatedDomainEventHandler(ISender sender, IEventBus eventBus)
    : IDomainEventHandler<TicketCreatedDomainEvent>
{
    public async Task Handle(TicketCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetTicketQuery(domainEvent.TicketId),
            cancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(GetTicketQuery), result.Error);

        await eventBus.PublishAsync(
            new TicketIssuedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                result.Value.Id,
                result.Value.CustomerId,
                result.Value.EventId,
                result.Value.Code),
            cancellationToken);
    }
}