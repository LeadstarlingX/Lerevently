using Lerevently.Modules.Events.Domain.Abstractions;

namespace Lerevently.Modules.Events.Domain.TicktTypes;


public sealed class TicketTypeCreatedDomainEvent(Guid ticketTypeId) : DomainEvent
{
    public Guid TicketTypeId { get; init; } = ticketTypeId;
}
