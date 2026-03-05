using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Carts;
using Lerevently.Modules.Ticketing.Application.Carts.AddItemToCart;
using Lerevently.Modules.Ticketing.Application.Carts.GetCart;
using Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class CartIsolationTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender _sender;

    [Before(Test)]
    public async Task SetupTest()
    {
        _scope = factory.Services.CreateScope();
        _sender = _scope.ServiceProvider.GetRequiredService<ISender>();
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
    public async Task Should_NotShowUserAItems_InUserBCart()
    {
        // Arrange
        var customerA = await RegisterAndGetCustomerAsync();
        var customerB = await RegisterAndGetCustomerAsync();
        
        var ticketTypeId = Guid.NewGuid();
        await _sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId, 100);

        // Act
        await _sender.Send(new AddItemToCartCommand(customerA, ticketTypeId, 5));

        // Assert
        Result<Cart> cartA = await _sender.Send(new GetCartQuery(customerA));
        Result<Cart> cartB = await _sender.Send(new GetCartQuery(customerB));

        cartA.Value.Items.Should().NotBeEmpty();
        cartB.Value.Items.Should().BeEmpty();
    }

    private async Task<Guid> RegisterAndGetCustomerAsync()
    {
        var command = new RegisterUserCommand(
            Faker.Internet.Email(), 
            Faker.Internet.Password(), 
            Faker.Name.FirstName(), 
            Faker.Name.LastName());
        var userResult = await _sender.Send(command);
        var customerResult = await Poller.WaitAsync(TimeSpan.FromSeconds(15), async () => await _sender.Send(new GetCustomerQuery(userResult.Value)));
        return customerResult.Value.Id;
    }
}