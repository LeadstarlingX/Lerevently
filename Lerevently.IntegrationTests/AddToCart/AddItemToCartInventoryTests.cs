using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Carts.AddItemToCart;
using Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class AddItemToCartInventoryTests : BaseIntegrationTest
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
    public async Task Should_ReturnError_WhenQuantityExceedsAvailableInventory()
    {
        // Arrange
        // 1. Register User
        var registerCommand = new RegisterUserCommand(
            Faker.Internet.Email(),
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        Result<Guid> userResult = await _sender.Send(registerCommand);
        userResult.IsSuccess.Should().BeTrue();

        // 2. Wait for Customer
        Result<CustomerResponse> customerResult = await Poller.WaitAsync(
            TimeSpan.FromSeconds(15),
            async () =>
            {
                var query = new GetCustomerQuery(userResult.Value);
                return await _sender.Send(query);
            });
        customerResult.IsSuccess.Should().BeTrue();

        // 3. Create Event with limited inventory (5 tickets)
        var ticketTypeId = Guid.NewGuid();
        await _sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId, 5);

        // Act
        // Try to add 6 tickets
        var command = new AddItemToCartCommand(customerResult.Value.Id, ticketTypeId, 6);
        Result result = await _sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}