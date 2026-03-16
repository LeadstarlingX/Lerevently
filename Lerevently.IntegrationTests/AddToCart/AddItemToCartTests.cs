using FluentAssertions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Carts.AddItemToCart;
using Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class AddItemToCartTests : BaseIntegrationTest
{
    private const decimal Quantity = 10;

    private IServiceScope _scope;
    private ISender _sender;

    [Before(Test)]
    public async Task Setup()
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
    public async Task Customer_ShouldBeAbleTo_AddItemToCart()
    {
        // Register user
        var command = new RegisterUserCommand(
            $"user-{Guid.NewGuid()}@test.com",
            Faker.Internet.Password(6),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        var userResult = await _sender.Send(command);

        userResult.IsSuccess.Should().BeTrue();

        // Get customer
        var customerResult = await Poller.WaitAsync(
            TimeSpan.FromSeconds(TimeForSpan),
            async () =>
            {
                var query = new GetCustomerQuery(userResult.Value);

                var customerResult = await _sender.Send(query);

                return customerResult;
            });

        customerResult.IsSuccess.Should().BeTrue();

        // Add item to cart
        var customer = customerResult.Value;
        var ticketTypeId = Guid.NewGuid();

        await _sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId, Quantity);

        var result = await _sender.Send(new AddItemToCartCommand(customer.Id, ticketTypeId, Quantity));

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}