using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Domain.Customers;
using Lerevently.Modules.Ticketing.Domain.Orders;
using Lerevently.Modules.Ticketing.Domain.Payments;
using Lerevently.Modules.Ticketing.UnitTests.Abstractions;

namespace Lerevently.Modules.Ticketing.UnitTests.Payments;

public class PaymentTests : BaseTest
{
    [Test]
    public async Task Create_ShouldRaiseDomainEvent_WhenPaymentIsCreated()
    {
        //Arrange
        var customer = Customer.Create(
            Guid.NewGuid(),
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        var order = Order.Create(customer);

        //Act
        Result<Payment> result = Payment.Create(
            order,
            Guid.NewGuid(),
            Faker.Random.Decimal(),
            Faker.Random.String(3));

        //Assert
        PaymentCreatedDomainEvent domainEvent =
            AssertDomainEventWasPublished<PaymentCreatedDomainEvent>(result.Value);

        await Assert.That(domainEvent.PaymentId).IsEqualTo(result.Value.Id);
    }

    [Test]
    public async Task Refund_ShouldReturnFailure_WhenAlreadyRefunded()
    {
        //Arrange
        decimal amount = Faker.Random.Decimal();

        var customer = Customer.Create(
            Guid.NewGuid(),
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        var order = Order.Create(customer);

        Result<Payment> paymentResult = Payment.Create(
            order,
            Guid.NewGuid(),
            amount,
            Faker.Random.String(3));

        Payment payment = paymentResult.Value;

        payment.Refund(amount);

        //Act
        Result result = payment.Refund(amount);

        //Assert
        await Assert.That(result.Error).IsEqualTo(PaymentErrors.AlreadyRefunded);
    }
}
