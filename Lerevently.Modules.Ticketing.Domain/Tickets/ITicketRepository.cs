using Lerevently.Modules.Ticketing.Domain.Events;

namespace Lerevently.Modules.Ticketing.Domain.Tickets;

public interface ITicketRepository
{
    Task<IEnumerable<Ticket>> GetForEventAsync(Event @event, CancellationToken cancellationToken = default);

    void InsertRange(IEnumerable<Ticket> tickets);
}
