using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using Lerevently.Modules.Users.Application.Users.UpdateUser;
using Lerevently.Modules.Users.Domain.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.UsersModule.Users;

public class UpdateUserTests : BaseIntegrationTest
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
    [MethodDataSource(typeof(TestDataSources), nameof(TestDataSources.AdditionTestData))]
    public async Task Should_ReturnError_WhenCommandIsNotValid(UpdateUserCommand command)
    {
        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Test]
    public async Task Should_ReturnError_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var updateResult = await Sender.Send(
            new UpdateUserCommand(userId, Faker.Name.FirstName(), Faker.Name.LastName()));

        // Assert
        updateResult.Error.Should().Be(UserErrors.NotFound(userId));
    }

    [Test]
    public async Task Should_ReturnSuccess_WhenUserExists()
    {
        // Arrange
        var result = await Sender.Send(new RegisterUserCommand(
            $"user-{Guid.NewGuid()}@test.com",
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName()));

        var userId = result.Value;

        // Act
        var updateResult = await Sender.Send(
            new UpdateUserCommand(userId, Faker.Name.FirstName(), Faker.Name.LastName()));

        // Assert
        updateResult.IsSuccess.Should().BeTrue();
    }

    public static class TestDataSources
    {
        public static IEnumerable<Func<UpdateUserCommand>> AdditionTestData()
        {
            yield return () => new UpdateUserCommand(Guid.Empty, Faker.Name.FirstName(), Faker.Name.LastName());
            yield return () => new UpdateUserCommand(Guid.NewGuid(), "", Faker.Name.LastName());
            yield return () => new UpdateUserCommand(Guid.NewGuid(), Faker.Name.FirstName(), "");
        }
    }
}