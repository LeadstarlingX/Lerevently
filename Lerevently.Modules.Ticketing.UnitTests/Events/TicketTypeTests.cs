using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Domain.Events;
using Lerevently.Modules.Ticketing.UnitTests.Abstractions;

namespace Lerevently.Modules.Ticketing.UnitTests.Events
{
    public class TicketTypeTests : BaseTest
    {
        [Test]
        public async Task Create_ShouldReturnValue_WhenTicketTypeIsCreated()
        {
            //Arrange
            DateTime startsAtUtc = DateTime.UtcNow;
            var @event = Event.Create(
                Guid.NewGuid(),
                Faker.Music.Genre(),
                Faker.Music.Genre(),
                Faker.Address.StreetAddress(),
                startsAtUtc,
                null);

            //Act
            Result<TicketType> result = TicketType.Create(
                Guid.NewGuid(),
                @event.Id,
                Faker.Name.FirstName(),
                Faker.Random.Decimal(),
                Faker.Random.String(3),
                Faker.Random.Decimal());

            //Assert
            await Assert.That(result.Value).IsNotNull();
        }

        [Test]
        public async Task UpdateQuantity_ShouldReturnFailure_WhenNotEnoughQuantity()
        {
            //Arrange
            DateTime startsAtUtc = DateTime.UtcNow;
            var @event = Event.Create(
                Guid.NewGuid(),
                Faker.Music.Genre(),
                Faker.Music.Genre(),
                Faker.Address.StreetAddress(),
                startsAtUtc,
                null);

            decimal quantity = Faker.Random.Decimal();
            var ticketType = TicketType.Create(
            Guid.NewGuid(),
            @event.Id,
            Faker.Name.FirstName(),
            Faker.Random.Decimal(),
            Faker.Random.String(3),
            quantity);

            //Act
            Result result = ticketType.UpdateQuantity(quantity + 1);

            //Assert
            await Assert.That(result.Error).IsEquivalentTo(TicketTypeErrors.NotEnoughQuantity(quantity));
        }

        [Test]
        public async Task UpdateQuantity_ShouldRaiseDomainEvent_WhenTicketTypesIsSoldOut()
        {
            //Arrange
            DateTime startsAtUtc = DateTime.UtcNow;
            var @event = Event.Create(
                Guid.NewGuid(),
                Faker.Music.Genre(),
                Faker.Music.Genre(),
                Faker.Address.StreetAddress(),
                startsAtUtc,
                null);

            decimal quantity = Faker.Random.Decimal();
            Result<TicketType> ticketType = TicketType.Create(
            Guid.NewGuid(),
            @event.Id,
            Faker.Name.FirstName(),
            Faker.Random.Decimal(),
            Faker.Random.String(3),
            quantity);

            //Act
            ticketType.Value.UpdateQuantity(quantity);

            //Assert
            var domainEvent = AssertDomainEventWasPublished<TicketTypeSoldOutDomainEvent>(ticketType.Value);

            await Assert.That(domainEvent.TicketTypeId).IsEqualTo(ticketType.Value.Id);
        }
    }
}
