using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Customers.CreateCustomer;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.TicketingModule.Customers;

public class CreateCustomerTests : BaseIntegrationTest
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
    public async Task Should_ReturnFailure_WhenCommandIsInvalid()
    {
        //Arrange
        var command = new CreateCustomerCommand(
            Guid.NewGuid(),
            string.Empty,
            string.Empty,
            string.Empty);

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task Should_CreateCustomer_WhenCommandIsInvalid()
    {
        //Arrange
        var command = new CreateCustomerCommand(
            Guid.NewGuid(),
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}