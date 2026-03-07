using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Customers.UpdateCustomer;
using Lerevently.Modules.Ticketing.Domain.Customers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.TicketingModule.Customers;

public class UpdateCustomerTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender Sender;

    [Before(Test)]
    public async Task SetupTest()
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
    public async Task Should_ReturnFailure_WhenCustomerDoesNotExist()
    {
        //Arrange
        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.Error.Should().Be(CustomerErrors.NotFound(command.CustomerId));
    }

    [Test]
    public async Task Should_ReturnSuccess_WhenCustomerIsUpdated()
    {
        //Arrange
        Guid customerId = await Sender.CreateCustomerAsync(Guid.NewGuid());

        var command = new UpdateCustomerCommand(
            customerId,
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}