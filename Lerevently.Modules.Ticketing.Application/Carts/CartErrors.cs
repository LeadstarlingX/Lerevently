using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Ticketing.Application.Carts;

public static class CartErrors
{
    public static readonly Error Empty = Error.Problem("Carts.Empty", "The cart is empty");
}
