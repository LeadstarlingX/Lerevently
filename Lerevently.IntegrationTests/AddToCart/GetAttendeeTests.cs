using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Attendance.Application.Attendees.GetAttendee;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class GetAttendeeTests : BaseIntegrationTest
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
    public async Task Should_ReturnError_WhenAttendeeDoesNotExist()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        Result<AttendeeResponse> result = await _sender.Send(new GetAttendeeQuery(nonExistentUserId));

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task Should_ReturnAttendee_WhenUserRegistered()
    {
        // Arrange
        var command = new RegisterUserCommand(
            Faker.Internet.Email(),
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        Result<Guid> userResult = await _sender.Send(command);
        userResult.IsSuccess.Should().BeTrue();

        // Act
        Result<AttendeeResponse> result = await Poller.WaitAsync(
            TimeSpan.FromSeconds(15),
            async () => await _sender.Send(new GetAttendeeQuery(userResult.Value)));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(command.Email);
        result.Value.FirstName.Should().Be(command.FirstName);
        result.Value.LastName.Should().Be(command.LastName);
    }
}