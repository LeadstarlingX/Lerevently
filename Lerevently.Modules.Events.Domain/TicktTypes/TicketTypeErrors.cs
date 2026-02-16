using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Events.Domain.TicktTypes;

public static class TicketTypeErrors
{
    public static Error NotFound(Guid ticketTypeId)
    {
        return Error.NotFound("TicketTypes.NotFound",
            $"The ticket type with the identifier {ticketTypeId} was not found");
    }
}