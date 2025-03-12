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
    public void Create_Should_Call_FinancialFactory_And_Repository_And_Return_Category()
    {
        // Arrange
        var categoryDto = _fixture.Create<CategoryDto>();
        var expectedCategory = _fixture.Create<Category>();

        _financialFactoryMock
            .Setup(ff => ff.CreateCategory(categoryDto))
            .Returns(expectedCategory);

        // Act
        var result = _categoryFacade.Create(categoryDto);

        // Assert
        Assert.Equal(expectedCategory, result);
        _financialFactoryMock.Verify(ff => ff.CreateCategory(categoryDto), Times.Once);
        _categoryRepositoryMock.Verify(cr => cr.Create(expectedCategory), Times.Once);
    }

    [Fact]
    public void GetById_Should_Return_Category_From_Repository()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedCategory = _fixture.Create<Category>();

        _categoryRepositoryMock
            .Setup(cr => cr.GetById(id))
            .Returns(expectedCategory);

        // Act
        var result = _categoryFacade.GetById(id);

        // Assert
        Assert.Equal(expectedCategory, result);
    }

    [Fact]
    public void EditCategory_Should_Call_Repository_Update_And_Return_Result()
    {
        // Arrange
        var editCategoryDto = _fixture.Create<EditCategoryDto>();
        _categoryRepositoryMock
            .Setup(cr => cr.Update(editCategoryDto))
            .Returns(true);

        // Act
        var result = _categoryFacade.EditCategory(editCategoryDto);

        // Assert
        Assert.True(result);
        _categoryRepositoryMock.Verify(cr => cr.Update(editCategoryDto), Times.Once);
    }

    [Fact]
    public void DeleteCategory_Should_Call_Repository_Delete_And_Return_Result()
    {
        // Arrange
        var id = Guid.NewGuid();
        _categoryRepositoryMock
            .Setup(cr => cr.Delete(id))
            .Returns(true);

        // Act
        var result = _categoryFacade.DeleteCategory(id);

        // Assert
        Assert.True(result);
        _categoryRepositoryMock.Verify(cr => cr.Delete(id), Times.Once);
    }

    [Fact]
    public void GetAllCategories_Should_Return_All_Categories_From_Repository()
    {
        // Arrange
        var categories = _fixture.CreateMany<Category>(3);
        _categoryRepositoryMock
            .Setup(cr => cr.GetAll())
            .Returns(categories);

        // Act
        var result = _categoryFacade.GetAllCategories();

        // Assert
        Assert.Equal(categories, result);
    }

    [Fact]
    public void GetByCondition_Should_Return_Filtered_Categories_From_Repository()
    {
        // Arrange
        Func<Category, bool> predicate = c => c.Id != Guid.Empty;
        var categories = _fixture.CreateMany<Category>(3);
        _categoryRepositoryMock
            .Setup(cr => cr.GetByCondition(predicate))
            .Returns(categories);

        // Act
        var result = _categoryFacade.GetByCondition(predicate);

        // Assert
        Assert.Equal(categories, result);
    }

    [Fact]
    public void CategoryExists_Should_Return_Repository_Result()
    {
        // Arrange
        var id = Guid.NewGuid();
        _categoryRepositoryMock
            .Setup(cr => cr.Exists(id))
            .Returns(true);

        // Act
        var result = _categoryFacade.CategoryExists(id);

        // Assert
        Assert.True(result);
    }
}