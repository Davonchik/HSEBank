using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories;

namespace TestHSEBank;

public class CategoryRepositoryTests
{
    private readonly CategoryRepository _repository;
    private readonly Fixture _fixture;

    public CategoryRepositoryTests()
    {
        _repository = new CategoryRepository();
        _fixture = new Fixture();
    }

    [Fact]
    public void Create_Should_Add_Category_To_Repository()
    {
        // Arrange
        var category = _fixture.Create<Category>();

        // Act
        var createdCategory = _repository.Create(category);

        // Assert
        Assert.Equal(category, createdCategory);
        Assert.True(_repository.Exists(category.CategoryId));
    }

    [Fact]
    public void GetById_Should_Return_Category_When_It_Exists()
    {
        // Arrange
        var category = _fixture.Create<Category>();
        _repository.Create(category);

        // Act
        var retrievedCategory = _repository.GetById(category.CategoryId);

        // Assert
        Assert.Equal(category, retrievedCategory);
    }

    [Fact]
    public void GetById_Should_Throw_KeyNotFoundException_When_Category_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => _repository.GetById(id));
        Assert.Equal("Категория не найдена", exception.Message);
    }

    [Fact]
    public void Update_Should_Return_True_And_Update_Category_When_It_Exists()
    {
        // Arrange
        var category = _fixture.Create<Category>();
        _repository.Create(category);
        var newName = "Updated Category Name";

        var editDto = _fixture.Build<EditCategoryDto>()
            .With(dto => dto.CategoryId, category.CategoryId)
            .With(dto => dto.Name, newName)
            .Create();

        // Act
        var result = _repository.Update(editDto);

        // Assert
        Assert.True(result);
        var updatedCategory = _repository.GetById(category.CategoryId);
        Assert.Equal(newName, updatedCategory.Name);
    }

    [Fact]
    public void Update_Should_Return_False_When_Category_Does_Not_Exist()
    {
        // Arrange
        var editDto = _fixture.Create<EditCategoryDto>();

        // Act
        var result = _repository.Update(editDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Delete_Should_Remove_Category_And_Return_True_When_It_Exists()
    {
        // Arrange
        var category = _fixture.Create<Category>();
        _repository.Create(category);

        // Act
        var result = _repository.Delete(category.CategoryId);

        // Assert
        Assert.True(result);
        Assert.False(_repository.Exists(category.CategoryId));
    }

    [Fact]
    public void Delete_Should_Return_False_When_Category_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = _repository.Delete(id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetAll_Should_Return_All_Categories()
    {
        // Arrange
        var categories = _fixture.CreateMany<Category>(3).ToList();
        foreach (var category in categories)
        {
            _repository.Create(category);
        }

        // Act
        var allCategories = _repository.GetAll().ToList();

        // Assert
        Assert.Equal(categories.Count, allCategories.Count);
        foreach (var category in categories)
        {
            Assert.Contains(category, allCategories);
        }
    }

    [Fact]
    public void GetByCondition_Should_Return_Filtered_Categories()
    {
        // Arrange
        var categories = _fixture.CreateMany<Category>(5).ToList();
        
        var targetCategory = categories.First();
        targetCategory.Name = "Target";
        foreach (var category in categories)
        {
            _repository.Create(category);
        }

        // Act
        var filteredCategories = _repository.GetByCondition(c => c.Name == "Target").ToList();

        // Assert
        Assert.Single(filteredCategories);
        Assert.Equal("Target", filteredCategories.First().Name);
    }

    [Fact]
    public void Exists_Should_Return_True_For_Existing_Category_And_False_For_Nonexistent()
    {
        // Arrange
        var category = _fixture.Create<Category>();
        _repository.Create(category);

        // Act & Assert
        Assert.True(_repository.Exists(category.CategoryId));
        Assert.False(_repository.Exists(Guid.NewGuid()));
    }
}