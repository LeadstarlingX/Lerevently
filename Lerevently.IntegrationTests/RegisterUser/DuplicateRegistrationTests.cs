using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.RegisterUser;

public sealed class DuplicateRegistrationTests : BaseIntegrationTest
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

    [Test]
    public async Task Should_ReturnError_WhenEmailIsAlreadyRegistered()
    {
        // Arrange
        var email = Faker.Internet.Email();
        var command = new RegisterUserCommand(
            email,
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        Result<Guid> firstResult = await _sender.Send(command);
        firstResult.IsSuccess.Should().BeTrue();

        // Act
        Result<Guid> secondResult = await _sender.Send(command);

        // Assert
        secondResult.IsFailure.Should().BeTrue();
    }
}