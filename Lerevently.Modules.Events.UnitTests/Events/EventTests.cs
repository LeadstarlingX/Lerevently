using FluentAssertions;
using Lerevently.Modules.Events.Domain.Categories;
using Lerevently.Modules.Events.Domain.Events;
using Lerevently.Modules.Events.UnitTests.Abstractions;

namespace Lerevently.Modules.Events.UnitTests.Events;

public class EventTests : BaseTest
{
    [Test]
    public void Create_ShouldReturnFailure_WhenEndDatePrecedesStartDate()
    {
        // Arrange
        var category = Category.Create(Faker.Music.Genre());
        var startsAtUtc = DateTime.Now;
        var endsAtUtc = startsAtUtc.AddMinutes(-1);

        // Act
        var result = Event.Create(
            category,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            endsAtUtc);

        // Assert
        result.Error.Should().Be(EventErrors.EndDatePrecedesStartDate);
    }

    [Test]
    public void Create_ShouldRaiseDomainEvent_WhenEventCreated()
    {
        // Arrange
        var category = Category.Create(Faker.Music.Genre());
        var startsAtUtc = DateTime.Now;

        // Act
        var result = Event.Create(
            category,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        var @event = result.Value;

        // Assert
        var domainEvent = AssertDomainEventWasPublished<EventCreatedDomainEvent>(@event);

        domainEvent.EventId.Should().Be(@event.Id);
    }

    [Test]
    public void Publish_ShouldReturnFailure_WhenEventNotDraft()
    {
        //Arrange
        var category = Category.Create(Faker.Music.Genre());
        var startsAtUtc = DateTime.UtcNow;

        var result = Event.Create(
            category,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        var @event = result.Value;

        @event.Publish();

        //Act
        var publishResult = @event.Publish();

        //Assert
        publishResult.Error.Should().Be(EventErrors.NotDraft);
    }


    [Test]
    public void Publish_ShouldRaiseDomainEvent_WhenEventPublished()
    {
        //Arrange
        var category = Category.Create(Faker.Music.Genre());
        var startsAtUtc = DateTime.UtcNow;

        var result = Event.Create(
            category,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        var @event = result.Value;

        //Act
        @event.Publish();

        //Assert
        var domainEvent =
            AssertDomainEventWasPublished<EventPublishedDomainEvent>(@event);

        domainEvent.EventId.Should().Be(@event.Id);
    }

    [Test]
    public void Reschedule_ShouldRaiseDomainEvent_WhenEventRescheduled()
    {
        //Arrange
        var category = Category.Create(Faker.Music.Genre());
        var startsAtUtc = DateTime.UtcNow;

        var result = Event.Create(
            category,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        var @event = result.Value;

        //Act
        @event.Reschedule(startsAtUtc.AddDays(1), startsAtUtc.AddDays(2));

        //Assert
        var domainEvent =
            AssertDomainEventWasPublished<EventRescheduledDomainEvent>(@event);

        domainEvent.EventId.Should().Be(@event.Id);
    }

    [Test]
    public void Cancel_ShouldRaiseDomainEvent_WhenEventCanceled()
    {
        //Arrange
        var category = Category.Create(Faker.Music.Genre());
        var startsAtUtc = DateTime.UtcNow;

        var result = Event.Create(
            category,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        var @event = result.Value;

        //Act
        @event.Cancel(startsAtUtc.AddMinutes(-1));

        //Assert
        var domainEvent =
            AssertDomainEventWasPublished<EventCanceledDomainEvent>(@event);

        domainEvent.EventId.Should().Be(@event.Id);
    }

    [Test]
    public void Cancel_ShouldReturnFailure_WhenEventAlreadyCanceled()
    {
        //Arrange
        var category = Category.Create(Faker.Music.Genre());
        var startsAtUtc = DateTime.UtcNow;

        var result = Event.Create(
            category,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        var @event = result.Value;

        @event.Cancel(startsAtUtc.AddMinutes(-1));

        //Act
        var cancelResult = @event.Cancel(startsAtUtc.AddMinutes(-1));

        //Assert
        cancelResult.Error.Should().Be(EventErrors.AlreadyCanceled);
    }

    [Test]
    public void Cancel_ShouldReturnFailure_WhenEventAlreadyStarted()
    {
        //Arrange
        var category = Category.Create(Faker.Music.Genre());
        var startsAtUtc = DateTime.UtcNow;

        var result = Event.Create(
            category,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        var @event = result.Value;

        //Act
        var cancelResult = @event.Cancel(startsAtUtc.AddMinutes(1));

        //Assert
        cancelResult.Error.Should().Be(EventErrors.AlreadyStarted);
    }
}