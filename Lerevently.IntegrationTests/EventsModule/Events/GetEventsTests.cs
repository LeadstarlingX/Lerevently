using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Events.Application.Events.GetEvents;
using Lerevently.Modules.Events.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.EventsModule.Events;

public class GetEventsTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender Sender;
    private EventsDbContext DbContext;

    [Before(Test)]
    public async Task SetupTest()
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<EventsDbContext>();
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
    public async Task Should_ReturnEvents_WhenEventsExist()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(Faker.Music.Genre());

        var eventId1 = await Sender.Events_CreateEventAsync(categoryId);
        var eventId2 = await Sender.Events_CreateEventAsync(categoryId);

        var query = new GetEventsQuery();

        // Act
        Result<IReadOnlyCollection<EventResponse>> result = await Sender.Send(query);

        // Assert
        await Assert.That(result.Value.FirstOrDefault(x => x.Id == eventId1)).IsNotNull();
        await Assert.That(result.Value.FirstOrDefault(x => x.Id == eventId2)).IsNotNull();
    }
    
    
}
