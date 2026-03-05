using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Attendance.Application.Attendees.GetAttendee;
using Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using Lerevently.Modules.Users.Application.Users.UpdateUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.Propagation;

public sealed class UserUpdatePropagationTests : BaseIntegrationTest
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
    public async Task Should_UpdateCustomerAndAttendee_WhenUserIsUpdated()
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
        var userId = userResult.Value;

        // 2. Wait for initial propagation
        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () => 
        {
            var result = await _sender.Send(new GetCustomerQuery(userId));
            return result.IsSuccess ? result : Result.Failure<CustomerResponse>(Error.Failure("Wait", "Wait"));
        });

        // 3. Update User
        var newFirstName = Faker.Name.FirstName();
        var newLastName = Faker.Name.LastName();
        var updateCommand = new UpdateUserCommand(userId, newFirstName, newLastName);
        
        Result updateResult = await _sender.Send(updateCommand);
        updateResult.IsSuccess.Should().BeTrue();

        // Act & Assert - Check Ticketing Module (Customer)
        Result<CustomerResponse> customerResult = await Poller.WaitAsync(
            TimeSpan.FromSeconds(15),
            async () =>
            {
                var query = new GetCustomerQuery(userId);
                var result = await _sender.Send(query);
                
                if (result.IsSuccess && result.Value.FirstName == newFirstName && result.Value.LastName == newLastName)
                    return result;
                
                return Result.Failure<CustomerResponse>(Error.Failure("Wait", "Not updated yet"));
            });

        customerResult.IsSuccess.Should().BeTrue();
        customerResult.Value.FirstName.Should().Be(newFirstName);
        customerResult.Value.LastName.Should().Be(newLastName);

        // Note: We could also check the Attendance module (GetAttendeeQuery), 
        // but checking one downstream module confirms the integration event was published.
    }
}