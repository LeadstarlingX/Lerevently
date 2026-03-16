using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Attendance.Domain.Attendees;
using Lerevently.Modules.Attendance.Domain.Events;
using Lerevently.Modules.Attendance.Domain.Tickets;
using Lerevently.Modules.Attendance.UnitTests.Abstractions;

namespace Lerevently.Modules.Attendance.UnitTests.Attendees;

public class AttendeeTests : BaseTest
{
    [Test]
    public async Task CheckIn_ShouldReturnFailure_WhenTicketIsNotValid()
    {
        //Arrange
        var attendee = Attendee.Create(
            Guid.NewGuid(),
            Faker.Internet.Email(),
            Faker.Person.FirstName,
            Faker.Person.LastName);

        var ticketAttendee = Attendee.Create(
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
            ticketAttendee,
            @event, 
            Faker.Random.String());

        //Act
        Result checkInAttendee = attendee.CheckIn(ticket);

        //Assert
        InvalidCheckInAttemptedDomainEvent domainEvent =
            AssertDomainEventWasPublished<InvalidCheckInAttemptedDomainEvent>(attendee);

        domainEvent.AttendeeId.Should().Be(attendee.Id);

        await Assert.That(checkInAttendee.Error).IsEqualTo(TicketErrors.InvalidCheckIn);

    }

    [Test]
    public async Task CheckIn_ShouldReturnFailure_WhenTicketAlreadyUsed()
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

        ticket.MarkAsUsed();

        //Act
        Result checkInAttendee = attendee.CheckIn(ticket);

        //Assert
        DuplicateCheckInAttemptedDomainEvent domainEvent =
            AssertDomainEventWasPublished<DuplicateCheckInAttemptedDomainEvent>(attendee);

        domainEvent.AttendeeId.Should().Be(attendee.Id);

        await Assert.That(checkInAttendee.Error).IsEqualTo(TicketErrors.DuplicateCheckIn);

    }

    [Test]
    public async Task CheckIn_ShouldRaiseDomainEvent_WhenSuccessfullyCheckedIn()
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
        attendee.CheckIn(ticket);

        //Assert
        AttendeeCheckedInDomainEvent domainEvent =
            AssertDomainEventWasPublished<AttendeeCheckedInDomainEvent>(attendee);

        await Assert.That(domainEvent.AttendeeId).IsEqualTo(attendee.Id);

    }
}
