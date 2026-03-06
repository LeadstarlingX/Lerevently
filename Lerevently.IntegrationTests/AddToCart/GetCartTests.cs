using FluentAssertions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Carts.AddItemToCart;
using Lerevently.Modules.Ticketing.Application.Carts.GetCart;
using Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class GetCartTests : BaseIntegrationTest
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
    public async Task Should_ReturnCartWithItems_WhenItemsAdded()
    {
        // Arrange
        // 1. Register User
        var registerCommand = new RegisterUserCommand(
            $"user-{Guid.NewGuid()}@test.com",
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        var userResult = await Sender.Send(registerCommand);
        userResult.IsSuccess.Should().BeTrue();

        // 2. Wait for Customer propagation
        var customerResult = await Poller.WaitAsync(
            TimeSpan.FromSeconds(TimeForSpan),
            async () =>
            {
                var query = new GetCustomerQuery(userResult.Value);
                return await Sender.Send(query);
            });
        customerResult.IsSuccess.Should().BeTrue();
        var customerId = customerResult.Value.Id;

        // 3. Create Event and Ticket Type
        var ticketTypeId = Guid.NewGuid();
        await Sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId, 100);

        // 4. Add Item to Cart
        var addCommand = new AddItemToCartCommand(customerId, ticketTypeId, 2);
        var addResult = await Sender.Send(addCommand);
        addResult.IsSuccess.Should().BeTrue();

        // Act
        var cartResult = await Sender.Send(new GetCartQuery(customerId));

        // Assert
        cartResult.IsSuccess.Should().BeTrue();
        cartResult.Value.Should().NotBeNull();
        cartResult.Value.Items.Should().ContainSingle();
        cartResult.Value.Items.First().Quantity.Should().Be(2);
    }
}