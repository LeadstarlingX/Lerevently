using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Events.RescheduleEvent;
using Lerevently.Modules.Ticketing.Domain.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.TicketingModule.Events;

public class RescheduleEventTests : BaseIntegrationTest
{
    private const decimal Quantity = 10;

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
        //Arrange
        var eventId = Guid.NewGuid();
        DateTime startsAtUtc = DateTime.UtcNow;
        DateTime endsAtUtc = startsAtUtc.AddHours(1);

        var command = new RescheduleEventCommand(eventId, startsAtUtc, endsAtUtc);

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.Error.Should().Be(EventErrors.NotFound(command.EventId));
    }

    [Test]
    public async Task Should_ReturnFailure_WhenEventAlreadyStarted()
    {
        //Arrange
        var eventId = Guid.NewGuid();
        var ticketTypeId = Guid.NewGuid();
        DateTime startsAtUtc = DateTime.UtcNow.AddMinutes(-5);
        DateTime endsAtUtc = startsAtUtc.AddHours(1);

        await Sender.CreateEventWithTicketTypeAsync(eventId, ticketTypeId, Quantity);

        var command = new RescheduleEventCommand(eventId, startsAtUtc, endsAtUtc);

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.Error.Should().Be(EventErrors.StartDateInPast);
    }

    [Test]
    public async Task Should_ReturnSuccess_WhenEventRescheduled()
    {
        //Arrange
        var eventId = Guid.NewGuid();
        var ticketTypeId = Guid.NewGuid();
        DateTime startsAtUtc = DateTime.UtcNow;
        DateTime endsAtUtc = startsAtUtc.AddHours(1);

        await Sender.CreateEventWithTicketTypeAsync(eventId, ticketTypeId, Quantity);

        var command = new RescheduleEventCommand(eventId, startsAtUtc, endsAtUtc);

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.Error.Should().Be(EventErrors.StartDateInPast);
    }
}
