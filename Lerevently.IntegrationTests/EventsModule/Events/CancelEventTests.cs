using FluentAssertions;
using Lerevently.Common.Application.Clock;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Events.Application.Events.CancelEvent;
using Lerevently.Modules.Events.Domain.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Lerevently.IntegrationTests.EventsModule.Events;

public class CancelEventTests : BaseIntegrationTest
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

        var command = new CancelEventCommand(eventId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(EventErrors.NotFound(eventId));
    }

    [Test]
    public async Task Should_ReturnFailure_WhenEventAlreadyCanceled()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(Faker.Music.Genre());
        Guid eventId = await Sender.Events_CreateEventAsync(categoryId);

        var command = new CancelEventCommand(eventId);

        await Sender.Send(command);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(EventErrors.AlreadyCanceled);
    }

    [Test]
    public async Task Should_ReturnFailure_WhenEventAlreadyStarted()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(Faker.Music.Genre());

        Guid eventId = await Sender.Events_CreateEventAsync(categoryId, DateTime.UtcNow.AddMinutes(5));
        
        var dateTimeProvider = _scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(DateTime.UtcNow.AddMinutes(15));

        var command = new CancelEventCommand(eventId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(EventErrors.AlreadyStarted);
    }

    [Test]
    public async Task Should_ReturnSuccess_WhenEventIsCanceled()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(Faker.Music.Genre());
        Guid eventId = await Sender.Events_CreateEventAsync(categoryId);

        var command = new CancelEventCommand(eventId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
