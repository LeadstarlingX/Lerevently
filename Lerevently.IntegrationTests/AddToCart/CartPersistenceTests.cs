using FluentAssertions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Carts.AddItemToCart;
using Lerevently.Modules.Ticketing.Application.Carts.GetCart;
using Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class CartPersistenceTests : BaseIntegrationTest
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
    public async ValueTask Teardown()
    {
        if (_scope is IAsyncDisposable asyncScope)
            await asyncScope.DisposeAsync();
        else
            _scope.Dispose();
    }

    [Test]
    public async Task Should_RetainItems_AcrossScopes()
    {
        // Arrange
        var customerId = await RegisterAndGetCustomerAsync();
        var ticketTypeId = Guid.NewGuid();
        await Sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId, 100);

        // Act
        await Sender.Send(new AddItemToCartCommand(customerId, ticketTypeId, 5));

        // Simulate new request/scope
        using var newScope = factory.Services.CreateScope();
        var newSender = newScope.ServiceProvider.GetRequiredService<ISender>();

        var cartResult = await newSender.Send(new GetCartQuery(customerId));

        // Assert
        cartResult.IsSuccess.Should().BeTrue();
        cartResult.Value.Items.Should().ContainSingle(i => i.Quantity == 5);
    }

    private async Task<Guid> RegisterAndGetCustomerAsync()
    {
        var command = new RegisterUserCommand(
            $"user-{Guid.NewGuid()}@test.com",
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());
        var userResult = await Sender.Send(command);

        await Assert.That(userResult.IsSuccess).IsTrue();

        var customerResult = await Poller.WaitAsync(TimeSpan.FromSeconds(TimeForSpan),
            async () => await Sender.Send(new GetCustomerQuery(userResult.Value)));

        await Assert.That(customerResult.IsSuccess).IsTrue();

        return customerResult.Value.Id;
    }
}