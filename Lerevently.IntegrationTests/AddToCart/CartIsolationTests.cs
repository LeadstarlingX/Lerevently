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
    public async Task Should_NotShowUserAItems_InUserBCart()
    {
        // Arrange
        var customerA = await RegisterAndGetCustomerAsync();
        var customerB = await RegisterAndGetCustomerAsync();
        
        var ticketTypeId = Guid.NewGuid();
        await Sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId, 100);

        // Act
        await Sender.Send(new AddItemToCartCommand(customerA, ticketTypeId, 5));

        // Assert
        Result<Cart> cartA = await Sender.Send(new GetCartQuery(customerA));
        Result<Cart> cartB = await Sender.Send(new GetCartQuery(customerB));

        cartA.Value.Items.Should().NotBeEmpty();
        cartB.Value.Items.Should().BeEmpty();
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
        
        var customerResult = await Poller.WaitAsync(TimeSpan.FromSeconds(TimeForSpan), async () => await Sender.Send(new GetCustomerQuery(userResult.Value)));
       
        await Assert.That(customerResult.IsSuccess).IsTrue();
        
        return customerResult.Value.Id;
    }
}