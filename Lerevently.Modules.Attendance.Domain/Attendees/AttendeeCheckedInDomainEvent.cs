using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Attendance.Domain.Attendees;

public sealed class AttendeeCheckedInDomainEvent(Guid attendeeId, Guid eventId) : DomainEvent
{
    public Guid AttendeeId { get; init; } = attendeeId;

    public Guid EventId { get; init; } = eventId;
}