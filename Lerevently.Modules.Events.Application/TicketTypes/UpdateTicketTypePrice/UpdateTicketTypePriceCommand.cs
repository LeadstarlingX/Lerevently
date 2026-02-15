using Lerevently.Modules.Events.Application.Abstractions.Messaging;

namespace Lerevently.Modules.Events.Application.TicketTypes.UpdateTicketTypePrice;

public sealed record UpdateTicketTypePriceCommand(Guid TicketTypeId, decimal Price) : ICommand;
