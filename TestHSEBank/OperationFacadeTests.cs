using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;
using Moq;

namespace TestHSEBank;

public class OperationFacadeTests
{
    private readonly Mock<IFinancialFactory> _financialFactoryMock;
    private readonly Mock<IOperationRepository> _operationRepositoryMock;
    private readonly Fixture _fixture;
    private readonly OperationFacade _operationFacade;

    public OperationFacadeTests()
    {
        _financialFactoryMock = new Mock<IFinancialFactory>();
        _operationRepositoryMock = new Mock<IOperationRepository>();
        _fixture = new Fixture();
        _operationFacade = new OperationFacade(_financialFactoryMock.Object, _operationRepositoryMock.Object);
    }

    [Fact]
    public void Create_Should_Call_FinancialFactory_And_Repository_And_Return_Operation()
    {
        // Arrange
        var operationDto = _fixture.Create<OperationDto>();
        var expectedOperation = _fixture.Create<Operation>();

        _financialFactoryMock
            .Setup(ff => ff.CreateOperation(operationDto))
            .Returns(expectedOperation);

        // Act
        var result = _operationFacade.Create(operationDto);

        // Assert
        Assert.Equal(expectedOperation, result);
        _financialFactoryMock.Verify(ff => ff.CreateOperation(operationDto), Times.Once);
        _operationRepositoryMock.Verify(or => or.Create(expectedOperation), Times.Once);
    }

    [Fact]
    public void GetById_Should_Return_Operation_From_Repository()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedOperation = _fixture.Create<Operation>();

        _operationRepositoryMock
            .Setup(or => or.GetById(id))
            .Returns(expectedOperation);

        // Act
        var result = _operationFacade.GetById(id);

        // Assert
        Assert.Equal(expectedOperation, result);
    }

    [Fact]
    public void EditOperation_Should_Call_Repository_Update_And_Return_Result()
    {
        // Arrange
        var editOperationDto = _fixture.Create<EditOperationDto>();
        _operationRepositoryMock
            .Setup(or => or.Update(editOperationDto))
            .Returns(true);

        // Act
        var result = _operationFacade.EditOperation(editOperationDto);

        // Assert
        Assert.True(result);
        _operationRepositoryMock.Verify(or => or.Update(editOperationDto), Times.Once);
    }

    [Fact]
    public void DeleteOperation_Should_Call_Repository_Delete_And_Return_Result()
    {
        // Arrange
        var id = Guid.NewGuid();
        _operationRepositoryMock
            .Setup(or => or.Delete(id))
            .Returns(true);

        // Act
        var result = _operationFacade.DeleteOperation(id);

        // Assert
        Assert.True(result);
        _operationRepositoryMock.Verify(or => or.Delete(id), Times.Once);
    }

    [Fact]
    public void GetAllOperations_Should_Return_All_Operations_From_Repository()
    {
        // Arrange
        var operations = _fixture.CreateMany<Operation>(3);
        _operationRepositoryMock
            .Setup(or => or.GetAll())
            .Returns(operations);

        // Act
        var result = _operationFacade.GetAllOperations();

        // Assert
        Assert.Equal(operations, result);
    }

    [Fact]
    public void GetByCondition_Should_Return_Filtered_Operations_From_Repository()
    {
        // Arrange
        Func<Operation, bool> predicate = op => op.Id != Guid.Empty;
        var operations = _fixture.CreateMany<Operation>(3);
        _operationRepositoryMock
            .Setup(or => or.GetByCondition(predicate))
            .Returns(operations);

        // Act
        var result = _operationFacade.GetByCondition(predicate);

        // Assert
        Assert.Equal(operations, result);
    }

    [Fact]
    public void OperationExists_Should_Return_Repository_Result()
    {
        // Arrange
        var id = Guid.NewGuid();
        _operationRepositoryMock
            .Setup(or => or.Exists(id))
            .Returns(true);

        // Act
        var result = _operationFacade.OperationExists(id);

        // Assert
        Assert.True(result);
    }
}