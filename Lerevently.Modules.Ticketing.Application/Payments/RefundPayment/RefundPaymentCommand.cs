using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Ticketing.Application.Payments.RefundPayment;

public sealed record RefundPaymentCommand(Guid PaymentId, decimal Amount) : ICommand;