using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Events.Application.Events.CreateEvent;
using Lerevently.Modules.Events.Domain.Categories;
using Lerevently.Modules.Events.Domain.Events;
using Lerevently.Modules.Events.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.EventsModule.Events;

public class CreateEventTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender Sender;
    private EventsDbContext DbContext;

    [Before(Test)]
    public async Task SetupTest()
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<EventsDbContext>();
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
    public async Task Should_ReturnFailure_WhenStartDateInPast()
    {
        // Arrange
        var command = new CreateEventCommand(
            Guid.NewGuid(),
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            DateTime.UtcNow.AddMinutes(-10),
            null);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var command = new CreateEventCommand(
            categoryId,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            DateTime.UtcNow.AddMinutes(10),
            null);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(CategoryErrors.NotFound(categoryId));
    }

    [Test]
    public async Task Should_ReturnFailure_WhenEndDatePrecedesStartDate()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        DateTime startsAtUtc = DateTime.UtcNow.AddMinutes(10);
        DateTime endsAtUtc = startsAtUtc.AddMinutes(-5);

        var command = new CreateEventCommand(
            categoryId,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            endsAtUtc);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Test]
    public async Task Should_CreateEvent_WhenCommandIsValid()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(Faker.Music.Genre());

        var command = new CreateEventCommand(
            categoryId,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            DateTime.UtcNow.AddMinutes(10),
            null);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
    
    
    
}
