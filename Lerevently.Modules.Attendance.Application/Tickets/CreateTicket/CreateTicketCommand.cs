using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Attendance.Application.Tickets.CreateTicket;

public sealed record CreateTicketCommand(Guid TicketId, Guid AttendeeId, Guid EventId, string Code) : ICommand;