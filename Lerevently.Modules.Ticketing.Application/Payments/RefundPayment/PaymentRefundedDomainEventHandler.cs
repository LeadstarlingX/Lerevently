using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Ticketing.Application.Abstractions.Payments;
using Lerevently.Modules.Ticketing.Domain.Payments;

namespace Lerevently.Modules.Ticketing.Application.Payments.RefundPayment;

internal sealed class PaymentRefundedDomainEventHandler(IPaymentService paymentService)
    : IDomainEventHandler<PaymentRefundedDomainEvent>
{
    public async Task Handle(PaymentRefundedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await paymentService.RefundAsync(domainEvent.TransactionId, domainEvent.RefundAmount);
    }
}