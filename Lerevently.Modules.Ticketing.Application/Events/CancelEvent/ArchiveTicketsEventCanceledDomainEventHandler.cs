using Lerevently.Common.Application.Exceptions;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Application.Tickets.ArchiveTicketsForEvent;
using Lerevently.Modules.Ticketing.Domain.Events;
using MediatR;

namespace Lerevently.Modules.Ticketing.Application.Events.CancelEvent;

internal sealed class ArchiveTicketsEventCanceledDomainEventHandler(ISender sender)
    : DomainEventHandler<EventCanceledDomainEvent>
{
    public override async Task Handle(EventCanceledDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new ArchiveTicketsForEventCommand(domainEvent.EventId), cancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(ArchiveTicketsForEventCommand), result.Error);
    }
}