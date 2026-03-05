using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Events.Application.Events.GetEvent;
using Lerevently.Modules.Ticketing.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class GetEventTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender _sender;
    private TicketingDbContext _dbContext;
    
    [Before(Test)]
    public async Task Setup()
    {
        _scope = factory.Services.CreateScope();
        _sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        _dbContext = _scope.ServiceProvider.GetRequiredService<TicketingDbContext>();
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
    public async Task Should_ReturnEvent_WhenEventExists()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        // Using the helper from BaseIntegrationTest/Extensions to simplify setup
        await _sender.CreateEventAsync(eventId, Guid.NewGuid(), 100);

        // Act
        var resultB = await Poller.WaitAsync(TimeSpan.FromSeconds(15), async () =>
        {
            var eventEntity = await _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(x => x.Id == eventId);
            
            if (eventEntity is null)
                return Result.Failure<Lerevently.Modules.Ticketing.Domain.Events.Event>(Error.Failure("Wait", "Wait"));
            
            return Result.Success(eventEntity);
        });
        
        // Assert
        resultB.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Should_ReturnError_WhenEventDoesNotExist()
    {
        // Act
        Result<EventResponse> result = await _sender.Send(new GetEventQuery(Guid.NewGuid()));

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}