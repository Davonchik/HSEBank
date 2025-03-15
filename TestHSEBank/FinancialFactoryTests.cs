using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services;

namespace TestHSEBank;

public class FinancialFactoryTests
{
    private readonly FinancialFactory _factory;
    private readonly Fixture _fixture;

    public FinancialFactoryTests()
    {
        _factory = new FinancialFactory();
        _fixture = new Fixture();
    }

    [Fact]
    public void CreateOperation_ShouldThrowException_WhenAmountIsNegative()
    {
        // Arrange
        var operationDto = _fixture.Create<OperationDto>();
        operationDto.Amount = -1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateOperation(operationDto));
        Assert.Equal("Amount cannot be negative", exception.Message);
    }

    [Fact]
    public void CreateOperation_ShouldReturnValidOperation_WhenAmountIsNonNegative()
    {
        // Arrange
        var operationDto = _fixture.Create<OperationDto>();
        operationDto.Amount = 100; // обеспечиваем неотрицательное значение

        // Act
        var result = _factory.CreateOperation(operationDto);

        // Assert
        Assert.Equal(operationDto.Amount, result.Amount);
        Assert.Equal(operationDto.Description, result.Description);
        Assert.Equal(operationDto.Type, result.Type);
        Assert.Equal(operationDto.BankAccountId, result.BankAccountId);
        Assert.Equal(operationDto.CategoryId, result.CategoryId);
        Assert.NotEqual(Guid.Empty, result.Id);
        // Проверяем, что дата создания близка к текущему времени (не более 5 секунд)
        Assert.True((DateTime.Now - result.Date).TotalSeconds < 5);
    }

    [Fact]
    public void CreateBankAccount_ShouldReturnValidBankAccount()
    {
        // Arrange
        var bankAccountDto = _fixture.Create<BankAccountDto>();

        // Act
        var result = _factory.CreateBankAccount(bankAccountDto);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(bankAccountDto.Name, result.Name);
        Assert.Equal(bankAccountDto.InitBalance, result.Balance);
    }

    [Fact]
    public void CreateCategory_ShouldReturnValidCategory()
    {
        // Arrange
        var categoryDto = _fixture.Create<CategoryDto>();

        // Act
        var result = _factory.CreateCategory(categoryDto);

        // Assert
        Assert.NotEqual(Guid.Empty, result.CategoryId);
        Assert.Equal(categoryDto.Name, result.Name);
        Assert.Equal(categoryDto.Type, result.Type);
    }
}