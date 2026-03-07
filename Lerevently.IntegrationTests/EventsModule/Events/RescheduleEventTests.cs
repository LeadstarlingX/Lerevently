using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Events.Application.Events.PublishEvent;
using Lerevently.Modules.Events.Application.Events.RescheduleEvent;
using Lerevently.Modules.Events.Domain.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.EventsModule.Events;

public class RescheduleEventTests : BaseIntegrationTest
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
    public async Task Should_ReturnFailure_WhenEventDoesNotExist()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        var command = new PublishEventCommand(eventId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(EventErrors.NotFound(eventId));
    }

    [Test]
    public async Task Should_ReturnFailure_WhenStartDateIsInPast()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(Faker.Music.Genre());
        Guid eventId = await Sender.Events_CreateEventAsync(categoryId);

        DateTime startsAtUtc = DateTime.UtcNow.AddMinutes(-5);

        var command = new RescheduleEventCommand(eventId, startsAtUtc, null);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(EventErrors.StartDateInPast);
    }

    [Test]
    public async Task Should_ReturnSuccess_WhenEventIsRescheduled()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(Faker.Music.Genre());
        Guid eventId = await Sender.Events_CreateEventAsync(categoryId);

        var command = new RescheduleEventCommand(eventId, DateTime.UtcNow.AddMinutes(10), null);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
