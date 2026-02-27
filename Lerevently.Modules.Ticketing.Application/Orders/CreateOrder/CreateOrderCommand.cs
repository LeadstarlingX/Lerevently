using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Ticketing.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(Guid CustomerId) : ICommand;