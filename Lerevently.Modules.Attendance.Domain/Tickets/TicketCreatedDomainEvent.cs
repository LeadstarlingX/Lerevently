using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Attendance.Domain.Tickets;

public sealed class TicketCreatedDomainEvent(Guid ticketId, Guid eventId) : DomainEvent
{
    public Guid TicketId { get; init; } = ticketId;

    public Guid EventId { get; init; } = eventId;
}