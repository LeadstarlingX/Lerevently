using FluentAssertions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.RegisterUser;

public sealed class DuplicateRegistrationTests : BaseIntegrationTest
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
    public async Task Should_ReturnError_WhenEmailIsAlreadyRegistered()
    {
        // Arrange
        var email = $"user-{Guid.NewGuid()}@test.com";
        var command = new RegisterUserCommand(
            email,
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        var firstResult = await Sender.Send(command);
        firstResult.IsSuccess.Should().BeTrue();

        // Act
        var secondResult = await Sender.Send(command);

        // Assert
        secondResult.IsFailure.Should().BeTrue();
    }
}