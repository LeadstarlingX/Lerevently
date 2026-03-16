using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Events.Application.TicketTypes.GetTicketType;
using Lerevently.Modules.Events.Application.TicketTypes.GetTicketTypes;
using Lerevently.Modules.Events.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.EventsModule.TicketTypes;

public class GetTicketTypesTests : BaseIntegrationTest
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
    public async Task Should_ReturnTicketTypes_WhenTicketTypesExists()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(Faker.Music.Genre());
        Guid eventId = await Sender.Events_CreateEventAsync(categoryId);

        await Sender.CreateTicketTypeAsync(eventId);
        await Sender.CreateTicketTypeAsync(eventId);

        var query = new GetTicketTypesQuery(eventId);

        // Act
        Result<IReadOnlyCollection<TicketTypeResponse>> result = await Sender.Send(query);

        // Assert
        result.Value.Should().HaveCount(2);
    }

}
