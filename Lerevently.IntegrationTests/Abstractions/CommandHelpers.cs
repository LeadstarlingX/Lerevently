using Bogus;
using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Attendance.Application.Attendees.CreateAttendee;
using Lerevently.Modules.Attendance.Application.Tickets.CreateTicket;
using Lerevently.Modules.Events.Application.Categories.CreateCategory;
using Lerevently.Modules.Events.Application.TicketTypes.CreateTicketType;
using Lerevently.Modules.Ticketing.Application.Customers.CreateCustomer;
using Lerevently.Modules.Ticketing.Application.Events.CreateEvent;
using MediatR;

using CreateEventCommand_Events = Lerevently.Modules.Events.Application.Events.CreateEvent.CreateEventCommand;
using CreateEventCommand_Attendance = Lerevently.Modules.Attendance.Application.Events.CreateEvent.CreateEventCommand;

namespace Lerevently.IntegrationTests.Abstractions;

internal static class CommandHelpers
{
    internal static async Task CreateEventAsync(
        this ISender sender,
        Guid eventId,
        Guid ticketTypeId,
        decimal quantity)
    {
        var faker = new Faker();

        var ticketType = new CreateEventCommand.TicketTypeRequest(
            ticketTypeId,
            eventId,
            faker.Music.Genre(),
            faker.Random.Decimal(),
            "USD",
            quantity);

        var result = await sender.Send(new CreateEventCommand(
            eventId,
            faker.Music.Genre(),
            faker.Music.Genre(),
            faker.Address.FullAddress(),
            DateTime.UtcNow,
            null,
            [ticketType]));

        result.IsSuccess.Should().BeTrue();
    }
    
    internal static async Task<Guid> CreateCustomerAsync(this ISender sender, Guid customerId)
    {
        var faker = new Faker();
        Result result = await sender.Send(
            new CreateCustomerCommand(
                customerId,
                faker.Internet.Email(),
                faker.Person.FirstName,
                faker.Person.LastName));

        result.IsSuccess.Should().BeTrue();

        return customerId;
    }

    internal static async Task CreateEventWithTicketTypeAsync(
        this ISender sender,
        Guid eventId,
        Guid ticketTypeId,
        decimal quantity)
    {
        var faker = new Faker();

        var ticketType = new CreateEventCommand.TicketTypeRequest(
            ticketTypeId,
            eventId,
            faker.Music.Genre(),
            faker.Random.Decimal(),
            "USD",
            quantity);

        Result result = await sender.Send(new CreateEventCommand(
            eventId,
            faker.Music.Genre(),
            faker.Music.Genre(),
            faker.Address.FullAddress(),
            DateTime.UtcNow,
            null,
            [ticketType]));

        result.IsSuccess.Should().BeTrue();
    }
    
    internal static async Task<Guid> CreateCategoryAsync(this ISender sender, string name)
    {
        Result<Guid> result = await sender.Send(new CreateCategoryCommand(name));

        return result.Value;
    }

    internal static async Task<Guid> Events_CreateEventAsync(
        this ISender sender,
        Guid categoryId,
        DateTime? startsAtUtc = null)
    {
        var faker = new Faker();
        Result<Guid> result = await sender.Send(
            new CreateEventCommand_Events(
                categoryId,
                faker.Music.Genre(),
                faker.Music.Genre(),
                faker.Address.StreetAddress(),
                startsAtUtc ?? DateTime.UtcNow.AddMinutes(10),
                null));

        return result.Value;
    }

    internal static async Task<Guid> CreateTicketTypeAsync(this ISender sender, Guid eventId)
    {
        var faker = new Faker();
        Result<Guid> result = await sender.Send(
            new CreateTicketTypeCommand(
                eventId,
                faker.Commerce.ProductName(),
                faker.Random.Decimal(),
                "USD",
                faker.Random.Decimal()));

        return result.Value;
    }
    
    internal static async Task<Guid> CreateAttendeeAsync(this ISender sender, Guid attendeeId)
    {
        var faker = new Faker();
        Result result = await sender.Send(
            new CreateAttendeeCommand(
                attendeeId, 
                faker.Internet.Email(),
                faker.Name.FirstName(),
                faker.Name.LastName()));

        result.IsSuccess.Should().BeTrue();

        return attendeeId;
    }

    internal static async Task<Guid> CreateTicketAsync(
        this ISender sender,
        Guid ticketId,
        Guid attendeeId,
        Guid eventId)
    {
        Result result = await sender.Send(
            new CreateTicketCommand(
                ticketId,
                attendeeId,
                eventId,
                Ulid.NewUlid().ToString()));

        result.IsSuccess.Should().BeTrue();

        return ticketId;
    }

    internal static async Task<Guid> CreateEventAsync(this ISender sender, Guid eventId)
    {
        var faker = new Faker();
        Result result = await sender.Send(
            new CreateEventCommand_Attendance(
                eventId, 
                faker.Music.Genre(),
                faker.Music.Genre(),
                faker.Address.StreetAddress(),
                DateTime.UtcNow.AddMinutes(10),
                null));

        result.IsSuccess.Should().BeTrue();

        return eventId; 
    }
    
}