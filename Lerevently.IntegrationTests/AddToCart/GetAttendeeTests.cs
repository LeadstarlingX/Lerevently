using FluentAssertions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Attendance.Application.Attendees.GetAttendee;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class GetAttendeeTests : BaseIntegrationTest
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
    public async Task Should_ReturnError_WhenAttendeeDoesNotExist()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var result = await Sender.Send(new GetAttendeeQuery(nonExistentUserId));

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task Should_ReturnAttendee_WhenUserRegistered()
    {
        // Arrange
        var command = new RegisterUserCommand(
            $"user-{Guid.NewGuid()}@test.com",
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        var userResult = await Sender.Send(command);
        userResult.IsSuccess.Should().BeTrue();

        // Act
        var result = await Poller.WaitAsync(
            TimeSpan.FromSeconds(TimeForSpan),
            async () => await Sender.Send(new GetAttendeeQuery(userResult.Value)));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(command.Email);
        result.Value.FirstName.Should().Be(command.FirstName);
        result.Value.LastName.Should().Be(command.LastName);
    }
}