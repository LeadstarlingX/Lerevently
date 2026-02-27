using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Application.Abstractions.Data;
using Lerevently.Modules.Ticketing.Application.Abstractions.Payments;
using Lerevently.Modules.Ticketing.Application.Carts;
using Lerevently.Modules.Ticketing.Domain.Customers;
using Lerevently.Modules.Ticketing.Domain.Events;
using Lerevently.Modules.Ticketing.Domain.Orders;
using Lerevently.Modules.Ticketing.Domain.Payments;

namespace Lerevently.Modules.Ticketing.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandHandler(
    ICustomerRepository customerRepository,
    IOrderRepository orderRepository,
    ITicketTypeRepository ticketTypeRepository,
    IPaymentRepository paymentRepository,
    IPaymentService paymentService,
    CartService cartService,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateOrderCommand>
{
    public async Task<Result> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var customer = await customerRepository.GetAsync(request.CustomerId, cancellationToken);

        if (customer is null) return Result.Failure(CustomerErrors.NotFound(request.CustomerId));

        var order = Order.Create(customer);

        var cart = await cartService.GetAsync(customer.Id, cancellationToken);

        if (!cart.Items.Any()) return Result.Failure(CartErrors.Empty);

        foreach (var cartItem in cart.Items)
        {
            // This acquires a pessimistic lock or throws an exception if already locked.
            var ticketType = await ticketTypeRepository.GetWithLockAsync(
                cartItem.TicketTypeId,
                cancellationToken);

            if (ticketType is null) return Result.Failure(TicketTypeErrors.NotFound(cartItem.TicketTypeId));

            var result = ticketType.UpdateQuantity(cartItem.Quantity);

            if (result.IsFailure) return Result.Failure(result.Error);

            order.AddItem(ticketType, cartItem.Quantity, cartItem.Price, ticketType.Currency);
        }

        orderRepository.Insert(order);

        // We're faking a payment gateway request here...
        var paymentResponse = await paymentService.ChargeAsync(order.TotalPrice, order.Currency);

        var payment = Payment.Create(
            order,
            paymentResponse.TransactionId,
            paymentResponse.Amount,
            paymentResponse.Currency);

        paymentRepository.Insert(payment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        await cartService.ClearAsync(customer.Id, cancellationToken);

        return Result.Success();
    }
}