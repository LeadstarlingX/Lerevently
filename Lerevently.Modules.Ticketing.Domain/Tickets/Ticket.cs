using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Domain.Events;
using Lerevently.Modules.Ticketing.Domain.Orders;

namespace Lerevently.Modules.Ticketing.Domain.Tickets;

public sealed class Ticket : Entity
{
#pragma warning disable CS8618
    private Ticket()
    {
    }
#pragma warning restore CS8618

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid OrderId { get; private set; }

    public Guid EventId { get; private set; }

    public Guid TicketTypeId { get; private set; }

    public string Code { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public bool Archived { get; private set; }

    public static Ticket Create(Order order, TicketType ticketType)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            CustomerId = order.CustomerId,
            OrderId = order.Id,
            EventId = ticketType.EventId,
            TicketTypeId = ticketType.Id,
            Code = $"tc_{Ulid.NewUlid()}",
            CreatedAtUtc = DateTime.UtcNow
        };

        ticket.Raise(new TicketCreatedDomainEvent(ticket.Id));

        return ticket;
    }

    public void Archive()
    {
        if (Archived) return;

        Archived = true;

        Raise(new TicketArchivedDomainEvent(Id, Code));
    }
}