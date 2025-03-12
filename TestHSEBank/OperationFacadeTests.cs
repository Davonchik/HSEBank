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
        _financialFactoryMock.Setup(f => f.CreateOperation(operationDto)).Returns(expectedOperation);
        _operationRepositoryMock.Setup(r => r.Create(expectedOperation)).Returns(expectedOperation);
        
        // Act
        var result = _operationFacade.Create(operationDto);
        
        // Assert
        Assert.Equal(expectedOperation, result);
        _financialFactoryMock.Verify(f => f.CreateOperation(operationDto), Times.Once);
        _operationRepositoryMock.Verify(r => r.Create(expectedOperation), Times.Once);
    }
    
    [Fact]
    public void GetById_Should_Throw_Exception_When_Operation_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _operationRepositoryMock.Setup(r => r.Exists(id)).Returns(false);
        
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _operationFacade.GetById(id));
        Assert.Equal($"Operation with id {id} does not exist", ex.Message);
    }
    
    [Fact]
    public void GetById_Should_Return_Operation_When_It_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedOperation = _fixture.Create<Operation>();
        _operationRepositoryMock.Setup(r => r.Exists(id)).Returns(true);
        _operationRepositoryMock.Setup(r => r.GetById(id)).Returns(expectedOperation);
        
        // Act
        var result = _operationFacade.GetById(id);
        
        // Assert
        Assert.Equal(expectedOperation, result);
    }
    
    [Fact]
    public void EditOperation_Should_Throw_Exception_When_Operation_Does_Not_Exist()
    {
        // Arrange
        var editDto = _fixture.Create<EditOperationDto>();
        _operationRepositoryMock.Setup(r => r.Exists(editDto.OperationId)).Returns(false);
        
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _operationFacade.EditOperation(editDto));
        Assert.Equal($"Operation with id {editDto.OperationId} does not exist", ex.Message);
    }
    
    [Fact]
    public void EditOperation_Should_Return_True_When_Update_Succeeds()
    {
        // Arrange
        var editDto = _fixture.Create<EditOperationDto>();
        _operationRepositoryMock.Setup(r => r.Exists(editDto.OperationId)).Returns(true);
        _operationRepositoryMock.Setup(r => r.Update(editDto)).Returns(true);
        
        // Act
        var result = _operationFacade.EditOperation(editDto);
        
        // Assert
        Assert.True(result);
        _operationRepositoryMock.Verify(r => r.Update(editDto), Times.Once);
    }
    
    [Fact]
    public void DeleteOperation_Should_Throw_Exception_When_Operation_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _operationRepositoryMock.Setup(r => r.Exists(id)).Returns(false);
        
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _operationFacade.DeleteOperation(id));
        Assert.Equal($"Operation with id {id} does not exist", ex.Message);
    }
    
    [Fact]
    public void DeleteOperation_Should_Return_True_When_Delete_Succeeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        _operationRepositoryMock.Setup(r => r.Exists(id)).Returns(true);
        _operationRepositoryMock.Setup(r => r.Delete(id)).Returns(true);
        
        // Act
        var result = _operationFacade.DeleteOperation(id);
        
        // Assert
        Assert.True(result);
        _operationRepositoryMock.Verify(r => r.Delete(id), Times.Once);
    }
    
    [Fact]
    public void GetAllOperations_Should_Return_All_Operations()
    {
        // Arrange
        var operations = _fixture.CreateMany<Operation>(3);
        _operationRepositoryMock.Setup(r => r.GetAll()).Returns(operations);
        
        // Act
        var result = _operationFacade.GetAllOperations();
        
        // Assert
        Assert.Equal(operations, result);
    }
    
    [Fact]
    public void GetByCondition_Should_Throw_Exception_When_No_Operations_Found()
    {
        // Arrange
        Func<Operation, bool> predicate = op => op.Id != Guid.Empty;
        _operationRepositoryMock.Setup(r => r.GetByCondition(predicate)).Returns(new List<Operation>());
        
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _operationFacade.GetByCondition(predicate));
        Assert.Equal($"No operations found for condition {predicate}", ex.Message);
    }
    
    [Fact]
    public void GetByCondition_Should_Return_Filtered_Operations_When_Found()
    {
        // Arrange
        Func<Operation, bool> predicate = op => op.Id != Guid.Empty;
        var filteredOperations = _fixture.CreateMany<Operation>(2).ToList();
        _operationRepositoryMock.Setup(r => r.GetByCondition(predicate)).Returns(filteredOperations);
        
        // Act
        var result = _operationFacade.GetByCondition(predicate);
        
        // Assert
        Assert.Equal(filteredOperations, result);
    }
    
    [Fact]
    public void OperationExists_Should_Return_True_When_Operation_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _operationRepositoryMock.Setup(r => r.Exists(id)).Returns(true);
        
        // Act
        var result = _operationFacade.OperationExists(id);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void OperationExists_Should_Return_False_When_Operation_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _operationRepositoryMock.Setup(r => r.Exists(id)).Returns(false);
        
        // Act
        var result = _operationFacade.OperationExists(id);
        
        // Assert
        Assert.False(result);
    }
}