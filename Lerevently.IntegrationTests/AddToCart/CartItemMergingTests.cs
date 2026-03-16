using FluentAssertions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Carts.AddItemToCart;
using Lerevently.Modules.Ticketing.Application.Carts.GetCart;
using Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class CartItemMergingTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender Sender;

    [Before(Test)]
    public async Task Setup()
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
    }

    [After(Test)]
    public async ValueTask TeardownTest()
    {
        if (_scope is IAsyncDisposable asyncScope)
            await asyncScope.DisposeAsync();
        else
            _scope.Dispose();
    }

    [Test]
    public async Task Should_MergeQuantities_WhenAddingSameItemTwice()
    {
        // Arrange
        var customerId = await RegisterAndGetCustomerAsync();
        var ticketTypeId = Guid.NewGuid();
        await Sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId, 100);

        // Act
        await Sender.Send(new AddItemToCartCommand(customerId, ticketTypeId, 5));
        var result = await Sender.Send(new AddItemToCartCommand(customerId, ticketTypeId, 5));

        // Assert
        result.IsSuccess.Should().BeTrue();

        var cartResult = await Sender.Send(new GetCartQuery(customerId));
        cartResult.Value.Items.Should().ContainSingle();
        cartResult.Value.Items.First().Quantity.Should().Be(10);
    }

    [Test]
    public async Task Should_KeepItemsSeparate_WhenAddingDifferentTicketTypes()
    {
        // Arrange
        var customerId = await RegisterAndGetCustomerAsync();
        var ticketTypeId1 = Guid.NewGuid();
        var ticketTypeId2 = Guid.NewGuid();
        await Sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId1, 100);
        await Sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId2, 100);

        // Act
        await Sender.Send(new AddItemToCartCommand(customerId, ticketTypeId1, 2));
        var result = await Sender.Send(new AddItemToCartCommand(customerId, ticketTypeId2, 3));

        // Assert
        result.IsSuccess.Should().BeTrue();

        var cartResult = await Sender.Send(new GetCartQuery(customerId));
        cartResult.Value.Items.Should().HaveCount(2);
        cartResult.Value.Items.Should().Contain(i => i.TicketTypeId == ticketTypeId1 && i.Quantity == 2);
        cartResult.Value.Items.Should().Contain(i => i.TicketTypeId == ticketTypeId2 && i.Quantity == 3);
    }

    private async Task<Guid> RegisterAndGetCustomerAsync()
    {
        var command = new RegisterUserCommand($"user-{Guid.NewGuid()}@test.com", Faker.Internet.Password(),
            Faker.Name.FirstName(), Faker.Name.LastName());
        var userResult = await Sender.Send(command);

        await Assert.That(userResult.IsSuccess).IsTrue();

        var customerResult = await Poller.WaitAsync(TimeSpan.FromSeconds(TimeForSpan),
            async () => await Sender.Send(new GetCustomerQuery(userResult.Value)));

        await Assert.That(customerResult.IsSuccess).IsTrue();

        return customerResult.Value.Id;
    }
}