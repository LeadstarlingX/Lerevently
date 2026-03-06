using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Attendance.Application.Attendees.GetAttendee;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using Lerevently.Modules.Users.Application.Users.UpdateUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class AttendeeUpdatePropagationTests : BaseIntegrationTest
{
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
    public async Task Should_UpdateAttendee_WhenUserIsUpdated()
    {
        // Arrange
        // 1. Register User
        var registerCommand = new RegisterUserCommand($"user-{Guid.NewGuid()}@test.com", Faker.Internet.Password(),
            Faker.Name.FirstName(), Faker.Name.LastName());
        var userResult = await _sender.Send(registerCommand);
        var userId = userResult.Value;

        // 2. Wait for initial propagation to Attendance
        await Poller.WaitAsync(TimeSpan.FromSeconds(TimeForSpan), async () =>
        {
            var result = await _sender.Send(new GetAttendeeQuery(userId));
            return result.IsSuccess ? result : Result.Failure<AttendeeResponse>(Error.Failure("Wait", "Wait"));
        });

        // 3. Update User
        var newFirstName = Faker.Name.FirstName();
        var newLastName = Faker.Name.LastName();
        var updateCommand = new UpdateUserCommand(userId, newFirstName, newLastName);

        var updateResult = await _sender.Send(updateCommand);
        updateResult.IsSuccess.Should().BeTrue();

        // Act & Assert - Check Attendance Module
        var attendeeResult = await Poller.WaitAsync(
            TimeSpan.FromSeconds(TimeForSpan),
            async () =>
            {
                var result = await _sender.Send(new GetAttendeeQuery(userId));
                if (result.IsSuccess && result.Value.FirstName == newFirstName && result.Value.LastName == newLastName)
                    return result;
                return Result.Failure<AttendeeResponse>(Error.Failure("Wait", "Not updated yet"));
            });

        attendeeResult.IsSuccess.Should().BeTrue();
        attendeeResult.Value.FirstName.Should().Be(newFirstName);
    }
}