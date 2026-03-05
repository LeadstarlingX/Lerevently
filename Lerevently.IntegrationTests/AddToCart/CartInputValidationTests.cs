using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Carts.AddItemToCart;
using Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class CartInputValidationTests : BaseIntegrationTest
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
    public async Task Should_ReturnError_WhenQuantityIsZero()
    {
        var customerId = await RegisterAndGetCustomerAsync();
        var ticketTypeId = Guid.NewGuid();
        await _sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId, 100);

        Result result = await _sender.Send(new AddItemToCartCommand(customerId, ticketTypeId, 0));
        
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task Should_ReturnError_WhenQuantityIsNegative()
    {
        var customerId = await RegisterAndGetCustomerAsync();
        var ticketTypeId = Guid.NewGuid();
        await _sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId, 100);

        Result result = await _sender.Send(new AddItemToCartCommand(customerId, ticketTypeId, -5));
        
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task Should_ReturnError_WhenTicketTypeDoesNotExist()
    {
        var customerId = await RegisterAndGetCustomerAsync();
        
        // Use a random TicketTypeId that hasn't been created
        Result result = await _sender.Send(new AddItemToCartCommand(customerId, Guid.NewGuid(), 1));
        
        result.IsFailure.Should().BeTrue();
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