using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Events.Application.Events.CreateEvent;
using Lerevently.Modules.Events.Application.Events.GetEvent;
using Lerevently.Modules.Events.Domain.Categories;
using Lerevently.Modules.Events.Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class CreateEventTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender Sender;
    private EventsDbContext Events_DbContext;
    
    [Before(Test)]
    public async Task Setup()
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        Events_DbContext = _scope.ServiceProvider.GetRequiredService<EventsDbContext>();
    }
    
    [After(Test)]
    public async ValueTask TeardownTest()
    {
        if (_scope is IAsyncDisposable asyncScope)
            await asyncScope.DisposeAsync();
        else
            _scope.Dispose();
    }

    [Test]
    public async Task Should_CreateEvent_WhenRequestIsValid()
    {
        // Arrange
        var category = Category.Create("Test Category");
        Events_DbContext.Categories.Add(category);
        await Events_DbContext.SaveChangesAsync();
        var categoryId = category.Id;
        
        var command = new CreateEventCommand(
            categoryId,
            Faker.Lorem.Sentence(),
            Faker.Lorem.Paragraph(),
            Faker.Address.City(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(4));
        
        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var eventId = result.Value;

        // Verify via Query
        Result<EventResponse> eventResult = await Sender.Send(new GetEventQuery(eventId));
        eventResult.IsSuccess.Should().BeTrue();
        eventResult.Value.Title.Should().Be(command.Title);
    }
}