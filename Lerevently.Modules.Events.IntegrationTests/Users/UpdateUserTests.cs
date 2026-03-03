using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Events.IntegrationTests.Abstractions;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using Lerevently.Modules.Users.Application.Users.UpdateUser;
using Lerevently.Modules.Users.Domain.Users;
using Lerevently.Modules.Users.Infrastructure.Database;
using Lerevently.Modules.Users.Infrastructure.Identity;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lerevently.Modules.Events.IntegrationTests.Users;

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
    protected ISender Sender;
    private static KeyCloakOptions _options;
    protected UsersDbContext DbContext;
    
    [Before(Test)]
    public async Task SetupTest()
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        _options = _scope.ServiceProvider.GetRequiredService<IOptions<KeyCloakOptions>>().Value;
        DbContext = _scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        // Fresh DbContext, clean state
        DbContext.Users.RemoveRange(DbContext.Users);
        await DbContext.SaveChangesAsync();
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
        Result result = await Sender.Send(command);

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
        Result updateResult = await Sender.Send(
            new UpdateUserCommand(userId, Faker.Name.FirstName(), Faker.Name.LastName()));

        // Assert
        updateResult.Error.Should().Be(UserErrors.NotFound(userId));
    }

    [Test]
    public async Task Should_ReturnSuccess_WhenUserExists()
    {
        // Arrange
        Result<Guid> result = await Sender.Send(new RegisterUserCommand(
            Faker.Internet.Email(),
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName()));

        Guid userId = result.Value;

        // Act
        Result updateResult = await Sender.Send(
            new UpdateUserCommand(userId, Faker.Name.FirstName(), Faker.Name.LastName()));

        // Assert
        updateResult.IsSuccess.Should().BeTrue();
    }
}
