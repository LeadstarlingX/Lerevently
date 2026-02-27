using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Ticketing.Domain.Events;

public static class EventErrors
{
    public static readonly Error StartDateInPast = Error.Problem(
        "Events.StartDateInPast",
        "The event start date is in the past");

    public static Error NotFound(Guid eventId)
    {
        return Error.NotFound("Events.NotFound", $"The event with the identifier {eventId} was not found");
    }
}