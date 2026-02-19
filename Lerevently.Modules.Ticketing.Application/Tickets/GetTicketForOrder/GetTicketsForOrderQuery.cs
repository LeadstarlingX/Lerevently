using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Application.Tickets.GetTicket;

namespace Lerevently.Modules.Ticketing.Application.Tickets.GetTicketForOrder;

public sealed record GetTicketsForOrderQuery(Guid OrderId) : IQuery<IReadOnlyCollection<TicketResponse>>;
