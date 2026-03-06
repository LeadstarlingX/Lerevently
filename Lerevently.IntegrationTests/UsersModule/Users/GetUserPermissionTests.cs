using FluentAssertions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Users.Application.Users.GetUserPermissions;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using Lerevently.Modules.Users.Domain.Users;
using Lerevently.Modules.Users.Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.UsersModule.Users;

public class GetUserPermissionTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender Sender;
    private UsersDbContext Users_DbContext;

    [Before(Test)]
    public async Task SetupTest()
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        Users_DbContext = _scope.ServiceProvider.GetRequiredService<UsersDbContext>();
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
        var identityId = Guid.NewGuid().ToString();

        // Act
        var permissionsResult = await Sender.Send(new GetUserPermissionsQuery(identityId));

        // Assert
        permissionsResult.Error.Should().Be(UserErrors.NotFound(identityId));
    }

    [Test]
    public async Task Should_ReturnPermissions_WhenUserExists()
    {
        // Arrange
        var result = await Sender.Send(new RegisterUserCommand(
            $"user-{Guid.NewGuid()}@test.com",
            Faker.Internet.Password(),
            Faker.Name.FirstName(),
            Faker.Name.LastName()));

        var identityId = Users_DbContext.Users.Single(u => u.Id == result.Value).IdentityId;

        // Act
        var permissionsResult = await Sender.Send(new GetUserPermissionsQuery(identityId));

        // Assert
        permissionsResult.IsSuccess.Should().BeTrue();
        permissionsResult.Value.Permissions.Should().NotBeEmpty();
    }
}