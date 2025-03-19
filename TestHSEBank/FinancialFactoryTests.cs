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
        var ex = Assert.Throws<ArgumentException>(() => _factory.CreateOperation(operationDto));
        Assert.Equal("Стоимость не может быть отрицательной!", ex.Message);
    }

    [Fact]
    public void CreateOperation_ShouldReturnValidOperation_WhenAmountIsNonNegative()
    {
        // Arrange
        var operationDto = _fixture.Create<OperationDto>();
        operationDto.Amount = 100; // неотрицательное значение

        // Act
        var result = _factory.CreateOperation(operationDto);

        // Assert
        Assert.Equal(operationDto.Amount, result.Amount);
        Assert.Equal(operationDto.Description, result.Description);
        Assert.Equal(operationDto.Type, result.Type);
        Assert.Equal(operationDto.BankAccountId, result.BankAccountId);
        Assert.Equal(operationDto.CategoryId, result.CategoryId);
        Assert.NotEqual(Guid.Empty, result.Id);
        // Проверяем, что дата создания близка к текущему времени (например, в пределах 5 секунд)
        Assert.True((DateTime.Now - result.Date).TotalSeconds < 5);
    }

    [Fact]
    public void CreateBankAccount_ShouldThrowException_WhenBalanceIsNegative()
    {
        // Arrange
        var bankAccountDto = _fixture.Create<BankAccountDto>();
        bankAccountDto.Balance = -100;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _factory.CreateBankAccount(bankAccountDto));
        Assert.Equal("Нельзя создать аккаунт с отрицательным балансом!", ex.Message);
    }

    [Fact]
    public void CreateBankAccount_ShouldReturnValidBankAccount_WhenBalanceIsNonNegative()
    {
        // Arrange
        var bankAccountDto = _fixture.Create<BankAccountDto>();
        bankAccountDto.Balance = 500; // неотрицательное значение

        // Act
        var result = _factory.CreateBankAccount(bankAccountDto);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(bankAccountDto.Name, result.Name);
        Assert.Equal(bankAccountDto.Balance, result.Balance);
    }

    [Fact]
    public void CreateCategory_ShouldReturnCategory_WithProvidedCategoryId()
    {
        // Arrange
        var categoryDto = _fixture.Create<CategoryDto>();
        var providedId = Guid.NewGuid();
        // Устанавливаем CategoryId в DTO
        categoryDto.CategoryId = providedId;

        // Act
        var result = _factory.CreateCategory(categoryDto);

        // Assert
        Assert.Equal(providedId, result.CategoryId);
        Assert.Equal(categoryDto.Name, result.Name);
        Assert.Equal(categoryDto.Type, result.Type);
    }

    [Fact]
    public void CreateCategory_ShouldGenerateNewCategoryId_WhenNotProvided()
    {
        // Arrange
        var categoryDto = _fixture.Create<CategoryDto>();
        // Оставляем CategoryId равным null
        categoryDto.CategoryId = null;

        // Act
        var result = _factory.CreateCategory(categoryDto);

        // Assert
        Assert.NotEqual(Guid.Empty, result.CategoryId);
        Assert.Equal(categoryDto.Name, result.Name);
        Assert.Equal(categoryDto.Type, result.Type);
    }
}