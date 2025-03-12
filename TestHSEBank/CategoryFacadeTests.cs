using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;
using Moq;

namespace TestHSEBank;

public class CategoryFacadeTests
{
    private readonly Mock<IFinancialFactory> _financialFactoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Fixture _fixture;
    private readonly CategoryFacade _categoryFacade;

    public CategoryFacadeTests()
    {
        _financialFactoryMock = new Mock<IFinancialFactory>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _fixture = new Fixture();
        _categoryFacade = new CategoryFacade(_financialFactoryMock.Object, _categoryRepositoryMock.Object);
    }

    [Fact]
    public void Create_Should_Return_New_Category_When_No_Duplicate_Found()
    {
        // Arrange
        var categoryDto = _fixture.Create<CategoryDto>();
        // Имитация отсутствия категории с таким же именем и типом
        _categoryRepositoryMock.Setup(r => r.GetAll())
            .Returns(new List<Category>());
        var expectedCategory = _fixture.Create<Category>();
        _financialFactoryMock.Setup(f => f.CreateCategory(categoryDto))
            .Returns(expectedCategory);
        _categoryRepositoryMock.Setup(r => r.Create(expectedCategory))
            .Returns(expectedCategory);

        // Act
        var result = _categoryFacade.Create(categoryDto);

        // Assert
        Assert.Equal(expectedCategory, result);
        _financialFactoryMock.Verify(f => f.CreateCategory(categoryDto), Times.Once);
        _categoryRepositoryMock.Verify(r => r.Create(expectedCategory), Times.Once);
    }

    [Fact]
    public void Create_Should_Throw_Exception_When_Duplicate_Category_Exists()
    {
        // Arrange
        var categoryDto = _fixture.Create<CategoryDto>();
        var duplicateCategory = _fixture.Create<Category>();
        // Имитация существования категории с таким же именем и типом
        duplicateCategory.Name = categoryDto.Name;
        duplicateCategory.Type = categoryDto.Type;
        _categoryRepositoryMock.Setup(r => r.GetAll())
            .Returns(new List<Category> { duplicateCategory });

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _categoryFacade.Create(categoryDto));
        Assert.Contains($"Category with name {categoryDto.Name} and type {categoryDto.Type} already exis", ex.Message);
    }

    [Fact]
    public void GetById_Should_Throw_Exception_When_Category_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _categoryRepositoryMock.Setup(r => r.Exists(id)).Returns(false);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _categoryFacade.GetById(id));
        Assert.Equal($"Category with id {id} does not exist", ex.Message);
    }

    [Fact]
    public void GetById_Should_Return_Category_When_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedCategory = _fixture.Create<Category>();
        _categoryRepositoryMock.Setup(r => r.Exists(id)).Returns(true);
        _categoryRepositoryMock.Setup(r => r.GetById(id)).Returns(expectedCategory);

        // Act
        var result = _categoryFacade.GetById(id);

        // Assert
        Assert.Equal(expectedCategory, result);
    }

    [Fact]
    public void EditCategory_Should_Throw_Exception_When_Category_Does_Not_Exist()
    {
        // Arrange
        var editDto = _fixture.Create<EditCategoryDto>();
        _categoryRepositoryMock.Setup(r => r.Exists(editDto.CategoryId)).Returns(false);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _categoryFacade.EditCategory(editDto));
        Assert.Equal($"Category with id {editDto.CategoryId} does not exist", ex.Message);
    }

    [Fact]
    public void EditCategory_Should_Return_True_When_Update_Succeeds()
    {
        // Arrange
        var editDto = _fixture.Create<EditCategoryDto>();
        _categoryRepositoryMock.Setup(r => r.Exists(editDto.CategoryId)).Returns(true);
        _categoryRepositoryMock.Setup(r => r.Update(editDto)).Returns(true);

        // Act
        var result = _categoryFacade.EditCategory(editDto);

        // Assert
        Assert.True(result);
        _categoryRepositoryMock.Verify(r => r.Update(editDto), Times.Once);
    }

    [Fact]
    public void DeleteCategory_Should_Throw_Exception_When_Category_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _categoryRepositoryMock.Setup(r => r.Exists(id)).Returns(false);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _categoryFacade.DeleteCategory(id));
        Assert.Equal($"Category with id {id} does not exist", ex.Message);
    }

    [Fact]
    public void DeleteCategory_Should_Return_True_When_Delete_Succeeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        _categoryRepositoryMock.Setup(r => r.Exists(id)).Returns(true);
        _categoryRepositoryMock.Setup(r => r.Delete(id)).Returns(true);

        // Act
        var result = _categoryFacade.DeleteCategory(id);

        // Assert
        Assert.True(result);
        _categoryRepositoryMock.Verify(r => r.Delete(id), Times.Once);
    }

    [Fact]
    public void GetAllCategories_Should_Return_All_Categories()
    {
        // Arrange
        var categories = _fixture.CreateMany<Category>(3);
        _categoryRepositoryMock.Setup(r => r.GetAll()).Returns(categories);

        // Act
        var result = _categoryFacade.GetAllCategories();

        // Assert
        Assert.Equal(categories, result);
    }

    [Fact]
    public void GetByCondition_Should_Throw_Exception_When_No_Categories_Found()
    {
        // Arrange
        Func<Category, bool> predicate = c => c.Name == "Nonexistent";
        _categoryRepositoryMock.Setup(r => r.GetByCondition(predicate))
            .Returns(new List<Category>());

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _categoryFacade.GetByCondition(predicate));
        Assert.Equal($"No operations found for condition {predicate}", ex.Message);
    }

    [Fact]
    public void GetByCondition_Should_Return_Filtered_Categories_When_Found()
    {
        // Arrange
        Func<Category, bool> predicate = c => c.Name.StartsWith("A");
        var filteredCategories = _fixture.CreateMany<Category>(2).ToList();
        _categoryRepositoryMock.Setup(r => r.GetByCondition(predicate))
            .Returns(filteredCategories);

        // Act
        var result = _categoryFacade.GetByCondition(predicate);

        // Assert
        Assert.Equal(filteredCategories, result);
    }

    [Fact]
    public void CategoryExists_Should_Return_True_When_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _categoryRepositoryMock.Setup(r => r.Exists(id)).Returns(true);

        // Act
        var result = _categoryFacade.CategoryExists(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CategoryExists_Should_Return_False_When_Not_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _categoryRepositoryMock.Setup(r => r.Exists(id)).Returns(false);

        // Act
        var result = _categoryFacade.CategoryExists(id);

        // Assert
        Assert.False(result);
    }
}