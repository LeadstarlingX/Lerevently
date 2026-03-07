using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Events.CancelEvent;
using Lerevently.Modules.Ticketing.Domain.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.TicketingModule.Events;

public class CancelEventTests : BaseIntegrationTest
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

        var command = new CancelEventCommand(eventId);

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.Error.Should().Be(EventErrors.NotFound(command.EventId));
    }

    [Test]
    public async Task Should_ReturnSuccess_WhenEventIsCanceled()
    {
        //Arrange
        var eventId = Guid.NewGuid();
        var ticketTypeId = Guid.NewGuid();

        await Sender.CreateEventWithTicketTypeAsync(eventId, ticketTypeId, Quantity);

        var command = new CancelEventCommand(eventId);

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}
