using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories;

namespace TestHSEBank;

public class OperationRepositoryTests
{
    private readonly OperationRepository _repository;
    private readonly Fixture _fixture;

    public OperationRepositoryTests()
    {
        _repository = new OperationRepository();
        _fixture = new Fixture();
    }

    [Fact]
    public void Create_Should_Add_Operation_To_Repository()
    {
        // Arrange
        var operation = _fixture.Create<Operation>();

        // Act
        var createdOperation = _repository.Create(operation);

        // Assert
        Assert.Equal(operation, createdOperation);
        Assert.True(_repository.Exists(operation.Id));
    }

    [Fact]
    public void GetById_Should_Return_Operation_When_It_Exists()
    {
        // Arrange
        var operation = _fixture.Create<Operation>();
        _repository.Create(operation);

        // Act
        var retrievedOperation = _repository.GetById(operation.Id);

        // Assert
        Assert.Equal(operation, retrievedOperation);
    }

    [Fact]
    public void GetById_Should_Throw_KeyNotFoundException_When_Operation_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.Throws<KeyNotFoundException>(() => _repository.GetById(id));
        Assert.Equal("Операция не найдена", ex.Message);
    }

    [Fact]
    public void Update_Should_Return_True_And_Update_CategoryId_When_Operation_Exists()
    {
        // Arrange
        var operation = _fixture.Create<Operation>();
        _repository.Create(operation);
        var newCategoryId = Guid.NewGuid();

        var editDto = _fixture.Build<EditOperationDto>()
            .With(dto => dto.OperationId, operation.Id)
            .With(dto => dto.CategoryId, newCategoryId)
            .Create();

        // Act
        var result = _repository.Update(editDto);

        // Assert
        Assert.True(result);
        var updatedOperation = _repository.GetById(operation.Id);
        Assert.Equal(newCategoryId, updatedOperation.CategoryId);
    }

    [Fact]
    public void Update_Should_Return_False_When_Operation_Does_Not_Exist()
    {
        // Arrange
        var editDto = _fixture.Create<EditOperationDto>();

        // Act
        var result = _repository.Update(editDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Delete_Should_Remove_Operation_And_Return_True_When_It_Exists()
    {
        // Arrange
        var operation = _fixture.Create<Operation>();
        _repository.Create(operation);

        // Act
        var result = _repository.Delete(operation.Id);

        // Assert
        Assert.True(result);
        Assert.False(_repository.Exists(operation.Id));
    }

    [Fact]
    public void Delete_Should_Return_False_When_Operation_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = _repository.Delete(id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetAll_Should_Return_All_Operations()
    {
        // Arrange
        var operations = _fixture.CreateMany<Operation>(3).ToList();
        foreach (var op in operations)
        {
            _repository.Create(op);
        }

        // Act
        var allOperations = _repository.GetAll().ToList();

        // Assert
        Assert.Equal(operations.Count, allOperations.Count);
        foreach (var op in operations)
        {
            Assert.Contains(op, allOperations);
        }
    }

    [Fact]
    public void GetByCondition_Should_Return_Filtered_Operations()
    {
        // Arrange
        var operations = _fixture.CreateMany<Operation>(5).ToList();
        // Для фильтрации установим у одной операции уникальное значение CategoryId
        var targetOperation = operations.First();
        var targetCategoryId = Guid.NewGuid();
        targetOperation.CategoryId = targetCategoryId;
        foreach (var op in operations)
        {
            _repository.Create(op);
        }

        // Act
        var filtered = _repository.GetByCondition(op => op.CategoryId == targetCategoryId).ToList();

        // Assert
        Assert.Single(filtered);
        Assert.Equal(targetCategoryId, filtered.First().CategoryId);
    }

    [Fact]
    public void Exists_Should_Return_True_When_Operation_Exists_And_False_When_Not()
    {
        // Arrange
        var operation = _fixture.Create<Operation>();
        _repository.Create(operation);

        // Act & Assert
        Assert.True(_repository.Exists(operation.Id));
        Assert.False(_repository.Exists(Guid.NewGuid()));
    }
}