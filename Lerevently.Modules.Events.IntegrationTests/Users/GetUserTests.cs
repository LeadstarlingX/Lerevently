using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Events.IntegrationTests.Abstractions;
using Lerevently.Modules.Users.Application.Users.GetUser;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using Lerevently.Modules.Users.Domain.Users;
using Lerevently.Modules.Users.Infrastructure.Database;
using Lerevently.Modules.Users.Infrastructure.Identity;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lerevently.Modules.Events.IntegrationTests.Users;

public class GetUserTests : BaseIntegrationTest
{
    
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
    public async Task Should_ReturnError_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        Result<UserResponse> userResult = await Sender.Send(new GetUserQuery(userId));

        // Assert
        userResult.Error.Should().Be(UserErrors.NotFound(userId));
    }

    [Test]
    public async Task Should_ReturnUser_WhenUserExists()
    {
        // Arrange
        var request = new RegisterUserCommand(
            Faker.Internet.Email(),
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());
        
        Result<Guid> result = await Sender.Send(request);
        
        await Assert.That(result.IsSuccess).IsTrue();
        Guid userId = result.Value;

        // Act
        Result<UserResponse> userResult = await Sender.Send(new GetUserQuery(userId));

        // Assert
        userResult.IsSuccess.Should().BeTrue();
        userResult.Value.Should().NotBeNull();
    }
}
