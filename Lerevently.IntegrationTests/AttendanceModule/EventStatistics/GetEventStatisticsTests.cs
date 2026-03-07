using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Attendance.Application.EventStatistics.GetEventStatistics;
using Lerevently.Modules.Attendance.Domain.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AttendanceModule.EventStatistics;

public class GetEventStatisticsTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender Sender;

    [Before(Test)]
    public async Task SetupTest()
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
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
    public async Task Should_ReturnFailure_WhenEventStatisticsDoesNotExist()
    {
        // Arrange
        var query = new GetEventStatisticsQuery(Guid.NewGuid());

        // Act
        Result<EventStatisticsResponse> result = await Sender.Send(query);

        // Assert
        result.Error.Should().Be(EventErrors.NotFound(query.EventId));
    }
}
