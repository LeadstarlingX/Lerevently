using FluentAssertions;
using Lerevently.Modules.Users.Domain.Users;
using Lerevently.Modules.Users.UnitTests.Abstractions;

namespace Lerevently.Modules.Users.UnitTests.Users;

public class UserTests : BaseTest
{
    [Test]
    public async Task Create_ShouldReturnUser()
    {
        // Act
        var user = User.Create(
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName(),
            Guid.NewGuid().ToString());

        // Assert
        await Assert.That(user).IsNotNull();
    }

    [Test]
    public async Task Create_ShouldReturnUser_WithMemberRole()
    {
        // Act
        var user = User.Create(
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName(),
            Guid.NewGuid().ToString());

        // Assert
        await Assert.That(user.Roles.Contains(Role.Member)).IsTrue();
    }

    [Test]
    public async Task Create_ShouldRaiseDomainEvent_WhenUserCreated()
    {
        // Act
        var user = User.Create(
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName(),
            Guid.NewGuid().ToString());

        // Assert
        UserRegisteredDomainEvent domainEvent =
            AssertDomainEventWasPublished<UserRegisteredDomainEvent>(user);

        await Assert.That(domainEvent.UserId).IsEqualTo(user.Id);
    }

    [Test]
    public async Task Update_ShouldRaiseDomainEvent_WhenUserUpdated()
    {
        // Arrange
        var user = User.Create(
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName(),
            Guid.NewGuid().ToString());

        // Act
        user.Update(user.LastName, user.FirstName);

        // Assert
        UserProfileUpdatedDomainEvent domainEvent =
            AssertDomainEventWasPublished<UserProfileUpdatedDomainEvent>(user);

        
        await Assert.That(domainEvent.UserId).IsEqualTo(user.Id);
        await Assert.That(domainEvent.FirstName).IsEqualTo(user.FirstName);
        await Assert.That(domainEvent.LastName).IsEqualTo(user.LastName);
    }

    [Test]
    public async Task Update_ShouldNotRaiseDomainEvent_WhenUserNotUpdated()
    {
        // Arrange
        var user = User.Create(
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName(),
            Guid.NewGuid().ToString());

        user.ClearDomainEvents();

        // Act
        user.Update(user.FirstName, user.LastName);

        // Assert
        await Assert.That(user.DomainEvents).IsEmpty();
    }
}
