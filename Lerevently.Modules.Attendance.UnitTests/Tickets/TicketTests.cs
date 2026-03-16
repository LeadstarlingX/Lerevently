using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Attendance.Domain.Attendees;
using Lerevently.Modules.Attendance.Domain.Events;
using Lerevently.Modules.Attendance.Domain.Tickets;
using Lerevently.Modules.Attendance.UnitTests.Abstractions;

namespace Lerevently.Modules.Attendance.UnitTests.Tickets;

public class TicketTests : BaseTest
{
    [Test]
    public async Task Create_ShouldRaiseDomainEvent_WhenTicketIsCreated()
    {
        //Arrange
        var attendee = Attendee.Create(
            Guid.NewGuid(), 
            Faker.Internet.Email(),
            Faker.Person.FirstName, 
            Faker.Person.LastName);

        DateTime startsAtUtc = DateTime.UtcNow;

        var @event = Event.Create(
            Guid.NewGuid(), 
            Faker.Music.Genre(), 
            Faker.Music.Genre(), 
            Faker.Address.StreetName(), 
            startsAtUtc, null);

        //Act
        Result<Ticket> result = Ticket.Create(
            Guid.NewGuid(),
            attendee,
            @event,
            Faker.Random.String());

        //Assert
        TicketCreatedDomainEvent domainEvent =
            AssertDomainEventWasPublished<TicketCreatedDomainEvent>(result.Value);

        await Assert.That(domainEvent.TicketId).IsEqualTo(result.Value.Id);

    }

    [Test]
    public async Task MarkAsUsed_ShouldRaiseDomainEvent_WhenTicketIsUsed()
    {
        //Arrange
        var attendee = Attendee.Create(
            Guid.NewGuid(), 
            Faker.Internet.Email(), 
            Faker.Person.FirstName, 
            Faker.Person.LastName);

        DateTime startsAtUtc = DateTime.UtcNow;

        var @event = Event.Create(
            Guid.NewGuid(), 
            Faker.Music.Genre(), 
            Faker.Music.Genre(), 
            Faker.Address.StreetName(),
            startsAtUtc, null);

        var ticket = Ticket.Create(
            Guid.NewGuid(),
            attendee,
            @event,
            Faker.Random.String());

        //Act
        ticket.MarkAsUsed();

        //Assert
        TicketUsedDomainEvent domainEvent =
            AssertDomainEventWasPublished<TicketUsedDomainEvent>(ticket);

        await Assert.That(domainEvent.TicketId).IsEqualTo(ticket.Id);

    }
}
