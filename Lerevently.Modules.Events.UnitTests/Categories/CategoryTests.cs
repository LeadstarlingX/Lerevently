using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Events.Domain.Categories;
using Lerevently.Modules.Events.UnitTests.Abstractions;

namespace Lerevently.Modules.Events.UnitTests.Categories;

public class CategoryTests : BaseTest
{
    [Test]
    public async Task Create_ShouldRaiseDomainEvent_WhenCategoryIsCreated()
    {
        //Act
        Result<Category> result = Category.Create(Faker.Music.Genre());

        //Assert
        var domainEvent =
            AssertDomainEventWasPublished<CategoryCreatedDomainEvent>(result.Value);

        domainEvent.CategoryId.Should().Be(result.Value.Id);
        await Assert.That(domainEvent.CategoryId).IsEqualTo(result.Value.Id);

    }

    [Test]
    public async Task Archive_ShouldRaiseDomainEvent_WhenCategoryIsArchived()
    {
        //Arrange
        Result<Category> result = Category.Create(Faker.Music.Genre());

        var category = result.Value;

        //Act
        category.Archive();

        //Assert
        var domainEvent =
            AssertDomainEventWasPublished<CategoryArchivedDomainEvent>(category);

        await Assert.That(domainEvent.CategoryId).IsEqualTo(category.Id);

        
    }

    [Test]
    public async Task ChangeName_ShouldRaiseDomainEvent_WhenCategoryNameIsChanged()
    {
        //Arrange
        Result<Category> result = Category.Create(Faker.Music.Genre());
        var category = result.Value;
        category.ClearDomainEvents();

        var newName = Faker.Music.Genre();

        //Act
        category.ChangeName(newName);

        //Assert
        var domainEvent =
            AssertDomainEventWasPublished<CategoryNameChangedDomainEvent>(category);

        await Assert.That(domainEvent.CategoryId).IsEqualTo(category.Id);

    }
}