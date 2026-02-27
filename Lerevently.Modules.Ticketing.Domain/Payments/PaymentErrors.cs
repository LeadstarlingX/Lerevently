using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Ticketing.Domain.Payments;

public static class PaymentErrors
{
    public static readonly Error AlreadyRefunded =
        Error.Problem("Payments.AlreadyRefunded", "The payment was already refunded");

    public static readonly Error NotEnoughFunds =
        Error.Problem("Payments.NotEnoughFunds", "There are not enough funds for a refund");

    public static Error NotFound(Guid paymentId)
    {
        return Error.NotFound("Payments.NotFound", $"The payment with the identifier {paymentId} was not found");
    }
}