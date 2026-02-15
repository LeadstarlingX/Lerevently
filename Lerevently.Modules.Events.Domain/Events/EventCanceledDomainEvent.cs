using Lerevently.Modules.Events.Domain.Abstractions;

namespace Lerevently.Modules.Events.Domain.Events;

public sealed class EventCanceledDomainEvent(Guid eventId) : DomainEvent
{
    public Guid EventId { get; init; } = eventId;
}