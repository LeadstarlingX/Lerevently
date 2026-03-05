using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.RegisterUser;

public sealed class GetCustomerTests : BaseIntegrationTest
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
    public async Task Should_ReturnError_WhenCustomerDoesNotExist()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        Result<CustomerResponse> result = await Sender.Send(new GetCustomerQuery(nonExistentUserId));

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task Should_ReturnCustomer_WhenUserRegistered()
    {
        // Arrange
        var command = new RegisterUserCommand(
            $"user-{Guid.NewGuid()}@test.com",
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        Result<Guid> userResult = await Sender.Send(command);
        userResult.IsSuccess.Should().BeTrue();

        // Act
        Result<CustomerResponse> result = await Poller.WaitAsync(
            TimeSpan.FromSeconds(TimeForSpan),
            async () => await Sender.Send(new GetCustomerQuery(userResult.Value)));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(command.Email);
        result.Value.FirstName.Should().Be(command.FirstName);
        result.Value.LastName.Should().Be(command.LastName);
    }
}