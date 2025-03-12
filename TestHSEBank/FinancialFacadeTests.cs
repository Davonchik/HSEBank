using System.Reflection;
using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.BusinessLogic.Shared;
using HSEBank.DataAccess.Models;
using HSEBank.Presentation;
using Moq;

namespace TestHSEBank;

public class FinancialFacadeTests
{
    private readonly Mock<IAccountFacade> _accountMock;
    private readonly Mock<ICategoryFacade> _categoryMock;
    private readonly Mock<IOperationFacade> _operationMock;
    private readonly Fixture _fixture;

    public FinancialFacadeTests()
    {
        // Сброс синглтона для независимости тестов.
        typeof(FinancialFacade)
            .GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)
            ?.SetValue(null, null);

        _accountMock = new Mock<IAccountFacade>();
        _categoryMock = new Mock<ICategoryFacade>();
        _operationMock = new Mock<IOperationFacade>();
        _fixture = new Fixture();
    }

    [Fact]
    public void Throw_Exception_If_No_Such_Category()
    {
        _categoryMock.Setup(f => f.CategoryExists(It.IsAny<Guid>())).Returns(false);
        _accountMock.Setup(f => f.AccountExists(It.IsAny<Guid>())).Returns(true);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        OperationDto operationDto = _fixture.Build<OperationDto>()
            .With(dto => dto.Type, OperationType.Expense)
            .Create<OperationDto>();

        Assert.Throws<ArgumentException>(() => financialFacade.CreateOperation(operationDto));
    }

    [Fact]
    public void Throw_Exception_If_No_Such_Account_On_CreateOperation()
    {
        _accountMock.Setup(f => f.AccountExists(It.IsAny<Guid>())).Returns(false);
        _categoryMock.Setup(f => f.CategoryExists(It.IsAny<Guid>())).Returns(true);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        OperationDto operationDto = _fixture.Create<OperationDto>();

        Assert.Throws<ArgumentException>(() => financialFacade.CreateOperation(operationDto));
    }

    [Fact]
    public void CreateOperation_Success()
    {
        OperationDto operationDto = _fixture.Create<OperationDto>();
        var expectedOperation = _fixture.Create<Operation>();

        _accountMock.Setup(f => f.AccountExists(operationDto.BankAccountId)).Returns(true);
        _categoryMock.Setup(f => f.CategoryExists(operationDto.CategoryId)).Returns(true);
        _operationMock.Setup(f => f.Create(operationDto)).Returns(expectedOperation);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.CreateOperation(operationDto);

        Assert.Equal(expectedOperation, result);
    }
    
    [Fact]
    public void EditOperation_Throws_Exception_When_Category_Not_Exist()
    {
        var editOperationDto = _fixture.Create<EditOperationDto>();
        _categoryMock.Setup(f => f.CategoryExists(editOperationDto.CategoryId)).Returns(false);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        Assert.Throws<ArgumentException>(() => financialFacade.EditOperation(editOperationDto));
    }

    [Fact]
    public void EditOperation_Success()
    {
        var editOperationDto = _fixture.Create<EditOperationDto>();
        _categoryMock.Setup(f => f.CategoryExists(editOperationDto.CategoryId)).Returns(true);
        _operationMock.Setup(f => f.EditOperation(editOperationDto)).Returns(true);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.EditOperation(editOperationDto);

        Assert.True(result);
    }

    [Fact]
    public void DeleteOperation_Throws_Exception_When_Operation_Not_Exist()
    {
        Guid operationId = Guid.NewGuid();
        _operationMock.Setup(f => f.OperationExists(operationId)).Returns(false);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        Assert.Throws<ArgumentException>(() => financialFacade.DeleteOperation(operationId));
    }

    [Fact]
    public void DeleteOperation_Success()
    {
        Guid operationId = Guid.NewGuid();
        _operationMock.Setup(f => f.OperationExists(operationId)).Returns(true);
        _operationMock.Setup(f => f.DeleteOperation(operationId)).Returns(true);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.DeleteOperation(operationId);

        Assert.True(result);
    }

    [Fact]
    public void GetOperation_Throws_Exception_When_Operation_Not_Exist()
    {
        Guid operationId = Guid.NewGuid();
        _operationMock.Setup(f => f.OperationExists(operationId)).Returns(false);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        Assert.Throws<ArgumentException>(() => financialFacade.GetOperation(operationId));
    }

    [Fact]
    public void GetOperation_Success()
    {
        Guid operationId = Guid.NewGuid();
        var expectedOperation = _fixture.Create<Operation>();

        _operationMock.Setup(f => f.OperationExists(operationId)).Returns(true);
        _operationMock.Setup(f => f.GetById(operationId)).Returns(expectedOperation);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.GetOperation(operationId);

        Assert.Equal(expectedOperation, result);
    }

    [Fact]
    public void CreateBankAccount_Throws_Exception_If_Account_Already_Exists()
    {
        var bankAccountDto = _fixture.Create<BankAccountDto>();
        _accountMock.Setup(f => f.AccountExists(bankAccountDto.AccountId)).Returns(true);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        Assert.Throws<ArgumentException>(() => financialFacade.CreateBankAccount(bankAccountDto));
    }

    [Fact]
    public void CreateBankAccount_Success()
    {
        var bankAccountDto = _fixture.Create<BankAccountDto>();
        var expectedAccount = _fixture.Create<BankAccount>();

        _accountMock.Setup(f => f.AccountExists(bankAccountDto.AccountId)).Returns(false);
        _accountMock.Setup(f => f.Create(bankAccountDto)).Returns(expectedAccount);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.CreateBankAccount(bankAccountDto);

        Assert.Equal(expectedAccount, result);
    }

    [Fact]
    public void EditBankAccount_Throws_Exception_If_Account_Does_Not_Exist()
    {
        var editBankAccountDto = _fixture.Create<EditBankAccountDto>();
        _accountMock.Setup(f => f.AccountExists(editBankAccountDto.BankAccountId)).Returns(false);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        Assert.Throws<ArgumentException>(() => financialFacade.EditBankAccount(editBankAccountDto));
    }

    [Fact]
    public void EditBankAccount_Success()
    {
        var editBankAccountDto = _fixture.Create<EditBankAccountDto>();
        _accountMock.Setup(f => f.AccountExists(editBankAccountDto.BankAccountId)).Returns(true);
        _accountMock.Setup(f => f.EditBankAccount(editBankAccountDto)).Returns(true);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.EditBankAccount(editBankAccountDto);

        Assert.True(result);
    }

    [Fact]
    public void DeleteBankAccount_Throws_Exception_If_Account_Does_Not_Exist()
    {
        Guid bankAccountId = Guid.NewGuid();
        _accountMock.Setup(f => f.AccountExists(bankAccountId)).Returns(false);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        Assert.Throws<ArgumentException>(() => financialFacade.DeleteBankAccount(bankAccountId));
    }

    [Fact]
    public void DeleteBankAccount_Success()
    {
        Guid bankAccountId = Guid.NewGuid();
        _accountMock.Setup(f => f.AccountExists(bankAccountId)).Returns(true);
        _accountMock.Setup(f => f.DeleteBankAccount(bankAccountId)).Returns(true);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.DeleteBankAccount(bankAccountId);

        Assert.True(result);
    }

    [Fact]
    public void GetBankAccount_Throws_Exception_If_Account_Does_Not_Exist()
    {
        Guid bankAccountId = Guid.NewGuid();
        _accountMock.Setup(f => f.AccountExists(bankAccountId)).Returns(false);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        Assert.Throws<ArgumentException>(() => financialFacade.GetBankAccount(bankAccountId));
    }

    [Fact]
    public void GetBankAccount_Success()
    {
        Guid bankAccountId = Guid.NewGuid();
        var expectedAccount = _fixture.Create<BankAccount>();

        _accountMock.Setup(f => f.AccountExists(bankAccountId)).Returns(true);
        _accountMock.Setup(f => f.GetById(bankAccountId)).Returns(expectedAccount);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.GetBankAccount(bankAccountId);

        Assert.Equal(expectedAccount, result);
    }

    [Fact]
    public void CreateCategory_Throws_Exception_If_Category_Already_Exists()
    {
        var categoryDto = _fixture.Create<CategoryDto>();
        _categoryMock.Setup(f => f.CategoryExists(categoryDto.CategoryId)).Returns(true);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        Assert.Throws<ArgumentException>(() => financialFacade.CreateCategory(categoryDto));
    }

    [Fact]
    public void CreateCategory_Success()
    {
        var categoryDto = _fixture.Create<CategoryDto>();
        var expectedCategory = _fixture.Create<Category>();

        _categoryMock.Setup(f => f.CategoryExists(categoryDto.CategoryId)).Returns(false);
        _categoryMock.Setup(f => f.Create(categoryDto)).Returns(expectedCategory);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.CreateCategory(categoryDto);

        Assert.Equal(expectedCategory, result);
    }

    [Fact]
    public void EditCategory_Success()
    {
        var editCategoryDto = _fixture.Create<EditCategoryDto>();
        _categoryMock.Setup(f => f.EditCategory(editCategoryDto)).Returns(true);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.EditCategory(editCategoryDto);

        Assert.True(result);
    }

    [Fact]
    public void DeleteCategory_Throws_Exception_If_Category_Does_Not_Exist()
    {
        Guid categoryId = Guid.NewGuid();
        _categoryMock.Setup(f => f.CategoryExists(categoryId)).Returns(false);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        Assert.Throws<ArgumentException>(() => financialFacade.DeleteCategory(categoryId));
    }

    [Fact]
    public void DeleteCategory_Success()
    {
        Guid categoryId = Guid.NewGuid();
        _categoryMock.Setup(f => f.CategoryExists(categoryId)).Returns(true);
        _categoryMock.Setup(f => f.DeleteCategory(categoryId)).Returns(true);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.DeleteCategory(categoryId);

        Assert.True(result);
    }

    [Fact]
    public void GetCategory_Throws_Exception_If_Category_Does_Not_Exist()
    {
        Guid categoryId = Guid.NewGuid();
        _categoryMock.Setup(f => f.CategoryExists(categoryId)).Returns(false);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        Assert.Throws<ArgumentException>(() => financialFacade.GetCategory(categoryId));
    }

    [Fact]
    public void GetCategory_Success()
    {
        Guid categoryId = Guid.NewGuid();
        var expectedCategory = _fixture.Create<Category>();

        _categoryMock.Setup(f => f.CategoryExists(categoryId)).Returns(true);
        _categoryMock.Setup(f => f.GetById(categoryId)).Returns(expectedCategory);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.GetCategory(categoryId);

        Assert.Equal(expectedCategory, result);
    }

    [Fact]
    public void RecalculateBalance_Throws_Exception_If_Account_Does_Not_Exist()
    {
        Guid bankAccountId = Guid.NewGuid();
        _accountMock.Setup(f => f.AccountExists(bankAccountId)).Returns(false);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        Assert.Throws<ArgumentException>(() => financialFacade.RecalculateBalance(bankAccountId));
    }

    [Fact]
    public void RecalculateBalance_Success()
    {
        Guid bankAccountId = Guid.NewGuid();
        // Предположим, что у нас есть операции: доход 100, расход 50, доход 30, итог = 80
        var operations = new List<Operation>
        {
            new Operation { BankAccountId = bankAccountId, Type = OperationType.Income, Amount = 100 },
            new Operation { BankAccountId = bankAccountId, Type = OperationType.Expense, Amount = 50 },
            new Operation { BankAccountId = bankAccountId, Type = OperationType.Income, Amount = 30 }
        };

        _accountMock.Setup(f => f.AccountExists(bankAccountId)).Returns(true);
        _operationMock.Setup(f => f.GetByCondition(It.IsAny<Func<Operation, bool>>()))
                      .Returns(operations);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var balance = financialFacade.RecalculateBalance(bankAccountId);

        Assert.Equal(80, balance);
    }

    [Fact]
    public void GetAllOperations_Success()
    {
        var operations = _fixture.CreateMany<Operation>(3);
        _operationMock.Setup(f => f.GetAllOperations()).Returns(operations);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.GetAllOperations();

        Assert.Equal(operations, result);
    }

    [Fact]
    public void GetAllBankAccounts_Success()
    {
        var accounts = _fixture.CreateMany<BankAccount>(3);
        _accountMock.Setup(f => f.GetAllBankAccounts()).Returns(accounts);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.GetAllBankAccounts();

        Assert.Equal(accounts, result);
    }

    [Fact]
    public void GetAllCategories_Success()
    {
        var categories = _fixture.CreateMany<Category>(3);
        _categoryMock.Setup(f => f.GetAllCategories()).Returns(categories);

        var financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        var result = financialFacade.GetAllCategories();

        Assert.Equal(categories, result);
    }
    
    //--------------
    
    [Fact]
    public void FinancialDataDto_DefaultProperties_AreNull()
    {
        // При создании объекта без инициализации свойства должны быть null
        var dto = new FinancialDataDto();

        Assert.Null(dto.BankAccounts);
        Assert.Null(dto.Categories);
        Assert.Null(dto.Operations);
    }

    [Fact]
    public void FinancialDataDto_PropertyAssignment_WorksCorrectly()
    {
        // Arrange - создаём тестовые данные
        var bankAccounts = new List<BankAccount>
        {
            new BankAccount { Id = Guid.NewGuid() }
        };

        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid() }
        };

        var operations = new List<Operation>
        {
            new Operation { Id = Guid.NewGuid() }
        };

        // Act - присваиваем значения свойствам
        var dto = new FinancialDataDto
        {
            BankAccounts = bankAccounts,
            Categories = categories,
            Operations = operations
        };

        // Assert - проверяем, что значения корректно установлены
        Assert.Equal(bankAccounts, dto.BankAccounts);
        Assert.Equal(categories, dto.Categories);
        Assert.Equal(operations, dto.Operations);
    }
}