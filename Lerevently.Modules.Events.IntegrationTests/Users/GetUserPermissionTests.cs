using FluentAssertions;
using Lerevently.Common.Application.Authorization;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Events.IntegrationTests.Abstractions;
using Lerevently.Modules.Users.Application.Users.GetUserPermissions;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using Lerevently.Modules.Users.Domain.Users;
using Lerevently.Modules.Users.Infrastructure.Database;
using Lerevently.Modules.Users.Infrastructure.Identity;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lerevently.Modules.Events.IntegrationTests.Users;

public class GetUserPermissionTests : BaseIntegrationTest
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
        string identityId = Guid.NewGuid().ToString();

        // Act
        Result<PermissionsResponse> permissionsResult = await Sender.Send(new GetUserPermissionsQuery(identityId));

        // Assert
        permissionsResult.Error.Should().Be(UserErrors.NotFound(identityId));
    }

    [Test]
    public async Task Should_ReturnPermissions_WhenUserExists()
    {
        // Arrange
        Result<Guid> result = await Sender.Send(new RegisterUserCommand(
            Faker.Internet.Email(),
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName()));

        string identityId = DbContext.Users.Single(u => u.Id == result.Value).IdentityId;

        // Act
        Result<PermissionsResponse> permissionsResult = await Sender.Send(new GetUserPermissionsQuery(identityId));

        // Assert
        permissionsResult.IsSuccess.Should().BeTrue();
        permissionsResult.Value.Permissions.Should().NotBeEmpty();
    }
}
