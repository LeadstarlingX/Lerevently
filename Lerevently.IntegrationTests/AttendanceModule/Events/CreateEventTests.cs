using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Attendance.Application.Events.CreateEvent;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AttendanceModule.Events;

public class CreateEventTests : BaseIntegrationTest
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
    
    public static class TestDataSources
    {
        public static IEnumerable<Func<(Guid, string, string, string, DateTime, DateTime?)>> AdditionTestData()
        {
            yield return () => ( Guid.Empty, Faker.Music.Genre(), Faker.Music.Genre(), Faker.Address.StreetAddress(), default, default );
            yield return () => ( Guid.NewGuid(), string.Empty, Faker.Music.Genre(), Faker.Address.StreetAddress(), default, default );
            yield return () => ( Guid.NewGuid(), Faker.Music.Genre(), string.Empty, Faker.Address.StreetAddress(), default, default  );
            yield return () => ( Guid.NewGuid(), Faker.Music.Genre(), Faker.Music.Genre(), string.Empty, default, default );
            yield return () => ( Guid.NewGuid(), Faker.Music.Genre(), Faker.Music.Genre(), Faker.Address.StreetAddress(), default, default );
        }
    }

    [Test]
    [MethodDataSource(typeof(TestDataSources), nameof(TestDataSources.AdditionTestData))]
    public async Task Should_ReturnFailure_WhenCommandIsInvalid(
        Guid eventId,
        string title,
        string description,
        string location,
        DateTime startsAtUtc,
        DateTime? endsAtUtc)
    {
        // Arrange
        var command = new CreateEventCommand(eventId, title, description, location, startsAtUtc, endsAtUtc);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task Should_ReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        var command = new CreateEventCommand(
            eventId,
            Faker.Music.Genre(),
            Faker.Music.Genre(),
            Faker.Address.StreetAddress(),
            DateTime.UtcNow.AddMinutes(10),
            null);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
