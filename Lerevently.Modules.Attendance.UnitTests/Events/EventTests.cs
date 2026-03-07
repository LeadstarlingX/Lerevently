using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Attendance.Domain.Events;
using Lerevently.Modules.Attendance.UnitTests.Abstractions;

namespace Lerevently.Modules.Attendance.UnitTests.Events;

public class EventTests : BaseTest
{
    [Test]
    public async Task Should_RaiseDomainEvent_WhenEventCreated()
    {
        //Arrange
        var eventId = Guid.NewGuid();
        DateTime startsAtUtc = DateTime.Now;

        //Act
        Result<Event> result = Event.Create(
            eventId,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        //Assert
        EventCreatedDomainEvent domainEvent =
            AssertDomainEventWasPublished<EventCreatedDomainEvent>(result.Value);

        await Assert.That(domainEvent.EventId).IsEqualTo(result.Value.Id);

    }
}
