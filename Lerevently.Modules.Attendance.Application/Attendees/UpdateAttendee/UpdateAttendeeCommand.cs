using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Attendance.Application.Attendees.UpdateAttendee;

public sealed record UpdateAttendeeCommand(Guid AttendeeId, string FirstName, string LastName) : ICommand;
