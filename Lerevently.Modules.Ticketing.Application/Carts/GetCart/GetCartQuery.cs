using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Ticketing.Application.Carts.GetCart;

public sealed record GetCartQuery(Guid CustomerId) : IQuery<Cart>;