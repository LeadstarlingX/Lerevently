using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Ticketing.Domain.Events;

public static class TicketTypeErrors
{
    public static Error NotFound(Guid ticketTypeId)
    {
        return Error.NotFound("TicketTypes.NotFound",
            $"The ticket type with the identifier {ticketTypeId} was not found");
    }

    public static Error NotEnoughQuantity(decimal availableQuantity)
    {
        return Error.Problem(
            "TicketTypes.NotEnoughQuantity",
            $"The ticket type has {availableQuantity} quantity available");
    }
}