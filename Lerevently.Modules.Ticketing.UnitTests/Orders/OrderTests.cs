using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Domain.Customers;
using Lerevently.Modules.Ticketing.Domain.Orders;
using Lerevently.Modules.Ticketing.UnitTests.Abstractions;

namespace Lerevently.Modules.Ticketing.UnitTests.Orders;

public class OrderTests : BaseTest
{
    [Test]
    public async Task Create_ShouldRaiseDomainEvent_WhenOrderIsCreated()
    {
        //Arrange
        var customer = Customer.Create(
            Guid.NewGuid(),
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        //Act
        Result<Order> result = Order.Create(customer);

        //Assert
        OrderCreatedDomainEvent domainEvent =
            AssertDomainEventWasPublished<OrderCreatedDomainEvent>(result.Value);

        await Assert.That(domainEvent.OrderId).IsEquivalentTo(result.Value.Id);
    }

    [Test]
    public async Task IssueTicket_ShouldReturnFailure_WhenTicketAlreadyIssued()
    {
        //Arrange
        var customer = Customer.Create(
            Guid.NewGuid(),
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        Result<Order> result = Order.Create(customer);

        result.Value.IssueTickets();
        Order order = result.Value;

        //Act
        Result issueTicketsResult = order.IssueTickets();

        //Assert
        await Assert.That(issueTicketsResult.IsFailure).IsTrue();
    }

    [Test]
    public async Task IssueTicket_ShouldRaiseDomainEvent_WhenTicketIsIssued()
    {
        //Arrange
        var customer = Customer.Create(
            Guid.NewGuid(),
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        Result<Order> result = Order.Create(customer);

        //Act
        result.Value.IssueTickets();

        //Assert
        OrderTicketsIssuedDomainEvent domainEvent =
            AssertDomainEventWasPublished<OrderTicketsIssuedDomainEvent>(result.Value);

        await Assert.That(domainEvent.OrderId).IsEquivalentTo(result.Value.Id);
    }
}
