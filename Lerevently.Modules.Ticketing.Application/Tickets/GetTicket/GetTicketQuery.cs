using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Ticketing.Application.Tickets.GetTicket;

public sealed record GetTicketQuery(Guid TicketId) : IQuery<TicketResponse>;
