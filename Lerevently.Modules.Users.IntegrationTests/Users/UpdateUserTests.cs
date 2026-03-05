using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using Lerevently.Modules.Users.Application.Users.UpdateUser;
using Lerevently.Modules.Users.Domain.Users;
using Lerevently.Modules.Users.IntegrationTests.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.Modules.Users.IntegrationTests.Users;

public class UpdateUserTests : BaseIntegrationTest
{
    public static class TestDataSources
    {
        public static IEnumerable<Func<UpdateUserCommand>> AdditionTestData()
        {
            yield return () => new UpdateUserCommand(Guid.Empty, Faker.Name.FirstName(), Faker.Name.LastName());
            yield return () => new UpdateUserCommand(Guid.NewGuid(), "", Faker.Name.LastName());
            yield return () => new UpdateUserCommand(Guid.NewGuid(), Faker.Name.FirstName(), "");
        }
    }
    
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
    [MethodDataSource(typeof(TestDataSources), nameof(TestDataSources.AdditionTestData))]
    public async Task Should_ReturnError_WhenCommandIsNotValid(UpdateUserCommand command)
    {
        // Act
        Result result = await _sender.Send(command);

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
        Result updateResult = await _sender.Send(
            new UpdateUserCommand(userId, Faker.Name.FirstName(), Faker.Name.LastName()));

        // Assert
        updateResult.Error.Should().Be(UserErrors.NotFound(userId));
    }

    [Test]
    public async Task Should_ReturnSuccess_WhenUserExists()
    {
        // Arrange
        Result<Guid> result = await _sender.Send(new RegisterUserCommand(
            Faker.Internet.Email(),
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName()));

        Guid userId = result.Value;

        // Act
        Result updateResult = await _sender.Send(
            new UpdateUserCommand(userId, Faker.Name.FirstName(), Faker.Name.LastName()));

        // Assert
        updateResult.IsSuccess.Should().BeTrue();
    }
}
