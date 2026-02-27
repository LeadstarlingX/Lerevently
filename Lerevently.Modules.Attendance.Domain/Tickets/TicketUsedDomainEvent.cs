using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Attendance.Domain.Tickets;

public sealed class TicketUsedDomainEvent(Guid ticketId) : DomainEvent
{
    public Guid TicketId { get; init; } = ticketId;
}