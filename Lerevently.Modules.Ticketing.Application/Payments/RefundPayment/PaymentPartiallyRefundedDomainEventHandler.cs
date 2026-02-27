using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Application.Abstractions.Payments;
using Lerevently.Modules.Ticketing.Domain.Payments;

namespace Lerevently.Modules.Ticketing.Application.Payments.RefundPayment;

internal sealed class PaymentPartiallyRefundedDomainEventHandler(IPaymentService paymentService)
    : IDomainEventHandler<PaymentPartiallyRefundedDomainEvent>
{
    public async Task Handle(PaymentPartiallyRefundedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await paymentService.RefundAsync(domainEvent.TransactionId, domainEvent.RefundAmount);
    }
}