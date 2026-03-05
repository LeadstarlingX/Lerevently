using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Events.Application.Events.CancelEvent;
using Lerevently.Modules.Events.Application.Events.GetEvent;
using Lerevently.Modules.Ticketing.Application.Carts.AddItemToCart;
using Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class EventCancellationTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender _sender;

    [Before(Test)]
    public async Task SetupTest()
    {
        _scope = factory.Services.CreateScope();
        _sender = _scope.ServiceProvider.GetRequiredService<ISender>();
    }

    [After(Test)]
    public async ValueTask TeardownTest()
    {
        if (_scope is IAsyncDisposable asyncScope)
            await asyncScope.DisposeAsync();
        else
            _scope.Dispose();
    }

    // [Test]
    // public async Task Should_UpdateEventStatus_WhenCancelled()
    // {
    //     // Arrange
    //     var eventId = Guid.NewGuid();
    //     await _sender.CreateEventAsync(eventId, Guid.NewGuid(), 100);
    //
    //     // Act
    //     Result result = await _sender.Send(new CancelEventCommand(eventId));
    //
    //     // Assert
    //     result.IsSuccess.Should().BeTrue();
    //     
    //     Result<EventResponse> eventResult = await _sender.Send(new GetEventQuery(eventId));
    //     
    //     eventResult.Value.Status.Should().Be("Cancelled"); // Assuming status string or enum
    // }

    // [Test]
    // public async Task Should_PreventAddToCart_WhenEventIsCancelled()
    // {
    //     TODO:
    //         Check this test again. 
    //
    //     // Arrange
    //     // 1. Setup User
    //     var registerCommand = new RegisterUserCommand(Faker.Internet.Email(), Faker.Internet.Password(), Faker.Name.FirstName(), Faker.Name.LastName());
    //     var userResult = await _sender.Send(registerCommand);
    //     var customerResult = await Poller.WaitAsync(TimeSpan.FromSeconds(15), async () => await _sender.Send(new GetCustomerQuery(userResult.Value)));
    //     
    //     // 2. Setup Event
    //     var eventId = Guid.NewGuid();
    //     var ticketTypeId = Guid.NewGuid();
    //     await _sender.CreateEventAsync(eventId, ticketTypeId, 100);
    //
    //     // 3. Cancel Event (Saga Trigger)
    //     await _sender.Send(new CancelEventCommand(eventId));
    //
    //     // 4. Wait for propagation (Saga: Event Cancelled -> Archive/Disable Ticket Types)
    //     await Task.Delay(16000); // Give saga time to process
    //
    //     // Act
    //     Result result = await _sender.Send(new AddItemToCartCommand(customerResult.Value.Id, ticketTypeId, 1));
    //
    //     // Assert
    //     result.IsFailure.Should().BeTrue();
    // }
}