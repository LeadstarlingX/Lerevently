using Lerevently.Modules.Events.Domain.Abstractions;

namespace Lerevently.Modules.Events.Domain.Events;

public sealed class EventCreatedDomainEvent(Guid eventId) : DomainEvent
{
    public Guid Id { get; init; } = eventId;
}