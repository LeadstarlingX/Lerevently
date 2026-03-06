using Lerevently.IntegrationTests.Abstractions;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class EventPublishingTests : BaseIntegrationTest
{
    // private IServiceScope _scope;
    // private ISender _sender;
    //
    // [Before(Test)]
    // public async Task Setup()
    // {
    //     _scope = factory.Services.CreateScope();
    //     _sender = _scope.ServiceProvider.GetRequiredService<ISender>();
    // }
    //
    // [After(Test)]
    // public async ValueTask TeardownTest()
    // {
    //     if (_scope is IAsyncDisposable asyncScope)
    //         await asyncScope.DisposeAsync();
    //     else
    //         _scope.Dispose();
    // }

    // [Test]
    // public async Task Should_PropagateToTicketing_WhenEventCreated()
    // {
    //      TODO:  
    //          Rewview this test
    //     // Arrange
    //     // 1. Setup User
    //     var registerCommand = new RegisterUserCommand($"user-{Guid.NewGuid()}@test.com", Faker.Internet.Password(), Faker.Name.FirstName(), Faker.Name.LastName());
    //     var userResult = await _sender.Send(registerCommand);
    //     var customerResult = await Poller.WaitAsync(TimeSpan.FromSeconds(15), async () => await _sender.Send(new GetCustomerQuery(userResult.Value)));
    //
    //     // 2. Create Event (Triggers Outbox -> Inbox -> Ticketing)
    //     var eventId = Guid.NewGuid();
    //     var ticketTypeId = Guid.NewGuid();
    //     var command = new CreateEventCommand(
    //         eventId,
    //         Faker.Lorem.Sentence(),
    //         Faker.Lorem.Paragraph(),
    //         Faker.Address.City(),
    //         DateTime.UtcNow.AddDays(1),
    //         DateTime.UtcNow.AddDays(1).AddHours(4));
    //
    //     await _sender.Send(command);
    //
    //     // Act & Assert - Try to add item to cart (relies on TicketType existing in Ticketing module)
    //     var result = await Poller.WaitAsync(TimeSpan.FromSeconds(15),
    //         async () => await _sender.Send(new AddItemToCartCommand(customerResult.Value.Id, ticketTypeId, 1)));
    //
    //     result.IsSuccess.Should().BeTrue();
    // }
}