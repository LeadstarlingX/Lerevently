using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Events.Application.TicketTypes.CreateTicketType;
using Lerevently.Modules.Events.Domain.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.EventsModule.TicketTypes;

public class CreateTicketTypeTests : BaseIntegrationTest
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
        var command = new CreateTicketTypeCommand(
            Guid.NewGuid(),
            Faker.Commerce.ProductName(),
            Faker.Random.Decimal(),
            "USD",
            Faker.Random.Decimal());

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(EventErrors.NotFound(command.EventId));
    }

    [Test]
    public async Task Should_CreateTicketType_WhenCommandIsValid()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(Faker.Music.Genre());
        Guid eventId = await Sender.Events_CreateEventAsync(categoryId);

        var command = new CreateTicketTypeCommand(
            eventId,
            Faker.Commerce.ProductName(),
            Faker.Random.Decimal(),
            "USD",
            Faker.Random.Decimal());

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}
