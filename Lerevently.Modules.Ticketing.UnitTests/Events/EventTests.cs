using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Domain.Events;
using Lerevently.Modules.Ticketing.UnitTests.Abstractions;

namespace Lerevently.Modules.Ticketing.UnitTests.Events;

public class EventTests : BaseTest
{
    [Test]
    public async Task Create_ShouldReturnValue_WhenEventIsCreated()
    {
        //Act
        Result<Event> @event = Event.Create(
            Guid.NewGuid(),
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            DateTime.UtcNow,
            null);

        //Assert
        await Assert.That(@event.Value).IsNotNull();
    }

    [Test]
    public async Task Reschedule_ShouldRaiseDomainEvent_WhenEventIsRescheduled()
    {
        //Arrange
        DateTime startsAtUtc = DateTime.UtcNow;

        Result<Event> @event = Event.Create(
            Guid.NewGuid(),
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        //Act
        @event.Value.Reschedule(
            startsAtUtc.AddDays(1),
            startsAtUtc.AddDays(2));

        //Assert
        EventRescheduledDomainEvent domainEvent =
            AssertDomainEventWasPublished<EventRescheduledDomainEvent>(@event.Value);

        await Assert.That(domainEvent.EventId).IsEqualTo(@event.Value.Id);
    }

    [Test]
    public async Task Cancel_ShouldRaiseDomainEvent_WhenEventIsCanceled()
    {
        //Arrange
        DateTime startsAtUtc = DateTime.UtcNow;

        Result<Event> @event = Event.Create(
            Guid.NewGuid(),
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        //Act
        @event.Value.Cancel();

        //Assert
        EventCanceledDomainEvent domainEvent =
            AssertDomainEventWasPublished<EventCanceledDomainEvent>(@event.Value);

        await Assert.That(domainEvent.EventId).IsEqualTo(@event.Value.Id);
    }

    [Test]
    public async Task PaymentsRefunded_ShouldRaiseDomainEvent_WhenPaymentsAreRefunded()
    {
        //Arrange
        DateTime startsAtUtc = DateTime.UtcNow;

        Result<Event> @event = Event.Create(
            Guid.NewGuid(),
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        //Act
        @event.Value.PaymentsRefunded();

        //Assert
        EventPaymentsRefundedDomainEvent domainEvent =
            AssertDomainEventWasPublished<EventPaymentsRefundedDomainEvent>(@event.Value);

        await Assert.That(domainEvent.EventId).IsEqualTo(@event.Value.Id);
    }

    [Test]
    public async Task TicketsArchived_ShouldRaiseDomainEvent_WhenTicketsAreArchived()
    {
        //Arrange
        DateTime startsAtUtc = DateTime.UtcNow;

        Result<Event> @event = Event.Create(
            Guid.NewGuid(),
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        //Act
        @event.Value.TicketsArchived();

        //Assert
        EventTicketsArchivedDomainEvent domainEvent =
            AssertDomainEventWasPublished<EventTicketsArchivedDomainEvent>(@event.Value);

        await Assert.That(domainEvent.EventId).IsEqualTo(@event.Value.Id);

    }
}
