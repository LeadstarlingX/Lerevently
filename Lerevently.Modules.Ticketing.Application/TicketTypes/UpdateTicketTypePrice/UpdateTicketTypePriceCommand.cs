using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Ticketing.Application.TicketTypes.UpdateTicketTypePrice;

public sealed record UpdateTicketTypePriceCommand(Guid TicketTypeId, decimal Price) : ICommand;
