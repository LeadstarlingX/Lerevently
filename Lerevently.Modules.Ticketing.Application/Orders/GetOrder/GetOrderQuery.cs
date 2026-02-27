using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Ticketing.Application.Orders.GetOrder;

public sealed record GetOrderQuery(Guid OrderId) : IQuery<OrderResponse>;