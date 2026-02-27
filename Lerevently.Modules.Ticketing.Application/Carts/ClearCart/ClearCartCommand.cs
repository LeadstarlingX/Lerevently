using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Ticketing.Application.Carts.ClearCart;

public sealed record ClearCartCommand(Guid CustomerId) : ICommand;