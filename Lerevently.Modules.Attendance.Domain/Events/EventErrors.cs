using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Attendance.Domain.Events;

public static class EventErrors
{
    public static Error NotFound(Guid eventId)
    {
        return Error.NotFound("Events.NotFound", $"The event with the identifier {eventId} was not found");
    }
}