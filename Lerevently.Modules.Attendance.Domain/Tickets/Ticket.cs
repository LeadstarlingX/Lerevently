using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Attendance.Domain.Attendees;
using Lerevently.Modules.Attendance.Domain.Events;

namespace Lerevently.Modules.Attendance.Domain.Tickets;

public sealed class Ticket : Entity
{
    
#pragma warning disable CS8618 
    private Ticket()
    {
    }
#pragma warning restore CS8618 

    public Guid Id { get; private set; }

    public Guid AttendeeId { get; private set; }

    public Guid EventId { get; private set; }

    public string Code { get; private set; }

    public DateTime? UsedAtUtc { get; private set; }

    public static Ticket Create(Guid ticketId, Attendee attendee, Event @event, string code)
    {
        var ticket = new Ticket
        {
            Id = ticketId,
            AttendeeId = attendee.Id,
            EventId = @event.Id,
            Code = code
        };

        ticket.Raise(new TicketCreatedDomainEvent(ticket.Id, ticket.EventId));

        return ticket;
    }

    internal void MarkAsUsed()
    {
        UsedAtUtc = DateTime.UtcNow;

        Raise(new TicketUsedDomainEvent(Id));
    }
}