using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Application.Tickets.GetTicket;

namespace Lerevently.Modules.Ticketing.Application.Tickets.GetTicketByCode;

public sealed record GetTicketByCodeQuery(string Code) : IQuery<TicketResponse>;