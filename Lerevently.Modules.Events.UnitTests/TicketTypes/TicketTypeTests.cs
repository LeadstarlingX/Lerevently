using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Events.Domain.Categories;
using Lerevently.Modules.Events.Domain.Events;
using Lerevently.Modules.Events.Domain.TicketTypes;
using Lerevently.Modules.Events.UnitTests.Abstractions;

namespace Lerevently.Modules.Events.UnitTests.TicketTypes;

public class TicketTypeTests : BaseTest
{
    [Test]
    public async Task Create_ShouldReturnValue_WhenTicketTypeIsCreated()
    {
        //Arrange
        var category = Category.Create(Faker.Music.Genre());
        var startsAtUtc = DateTime.UtcNow;

        var eventResult = Event.Create(
            category,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        //Act
        Result<TicketType> result = TicketType.Create(
            eventResult.Value,
            Faker.Music.Genre(),
            Faker.Random.Decimal(),
            Faker.Random.String(),
            Faker.Random.Decimal());

        //Assert
        await Assert.That(result.Value).IsNotNull();
    }

    [Test]
    public async Task UpdatePrice_ShouldRaiseDomainEvent_WhenTicketTypeIsUpdated()
    {
        //Arrange
        var category = Category.Create(Faker.Music.Genre());
        var startsAtUtc = DateTime.UtcNow;

        var eventResult = Event.Create(
            category,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            startsAtUtc,
            null);

        Result<TicketType> result = TicketType.Create(
            eventResult.Value,
            Faker.Music.Genre(),
            Faker.Random.Decimal(),
            Faker.Random.String(),
            Faker.Random.Decimal());

        var ticketType = result.Value;

        //Act
        ticketType.UpdatePrice(Faker.Random.Decimal());

        //Assert
        var domainEvent =
            AssertDomainEventWasPublished<TicketTypePriceChangedDomainEvent>(ticketType);

        await Assert.That(domainEvent.TicketTypeId).IsEqualTo(ticketType.Id);
    }
}