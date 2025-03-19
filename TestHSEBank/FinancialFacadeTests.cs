using System.Reflection;
using System.Text.Json;
using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.DataAccess.Models;
using HSEBank.Presentation;
using HSEBank.DataAccess.Common.Constants;
using Moq;
using Type = HSEBank.DataAccess.Common.Enums.Type;

namespace TestHSEBank;

public class TestBankAccount : BankAccount
{
    public new void Accept(IDataExportVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public class FinancialFacadeTests
{
    private readonly Mock<IAccountFacade> _accountFacadeMock;
    private readonly Mock<ICategoryFacade> _categoryFacadeMock;
    private readonly Mock<IOperationFacade> _operationFacadeMock;
    private readonly Mock<IAnalyticsService> _analyticsServiceMock;
    private readonly Fixture _fixture;
    private FinancialFacade _facade;

    public FinancialFacadeTests()
    {
        // Сброс синглтона для независимости тестов
        typeof(FinancialFacade)
            .GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)
            ?.SetValue(null, null);

        _accountFacadeMock = new Mock<IAccountFacade>();
        _categoryFacadeMock = new Mock<ICategoryFacade>();
        _operationFacadeMock = new Mock<IOperationFacade>();
        _analyticsServiceMock = new Mock<IAnalyticsService>();
        _fixture = new Fixture();
        _facade = FinancialFacade.GetInstance(
            _accountFacadeMock.Object,
            _categoryFacadeMock.Object,
            _operationFacadeMock.Object,
            _analyticsServiceMock.Object);
    }
    
    [Fact]
    public void ImportBankAccountsFromFile_CallsCreateBankAccount_ForEachDTO()
    {
        // Arrange: создаём список DTO для банковских счетов и сериализуем его в JSON.
        var bankAccountDto = _fixture.Create<BankAccountDto>();
        var dtos = new List<BankAccountDto> { bankAccountDto };
        string json = System.Text.Json.JsonSerializer.Serialize(dtos);
        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        File.WriteAllText(tempFile, json);
    
        // Настраиваем мок для создания банковского счёта.
        // Важно, чтобы он возвращал не null.
        var account = _fixture.Create<BankAccount>();
        account.Id = Guid.NewGuid(); // гарантируем, что Id задан.
        _accountFacadeMock.Setup(a => a.Create(It.IsAny<BankAccountDto>())).Returns(account);
        _accountFacadeMock.Setup(a => a.AccountExists(account.Id)).Returns(true);
    
        // Настраиваем начальную категорию. Предположим, что она уже существует.
        Guid initGuid = Guid.Parse(HSEBank.DataAccess.Common.Constants.Constants.InitCategoryId);
        _categoryFacadeMock.Setup(c => c.CategoryExists(initGuid)).Returns(true);
        var existingCategory = _fixture.Create<Category>();
        existingCategory.CategoryId = initGuid;
        _categoryFacadeMock.Setup(c => c.GetById(initGuid)).Returns(existingCategory);
        _categoryFacadeMock.Setup(c => c.CategoryExists(existingCategory.CategoryId)).Returns(true);
    
        // Настраиваем мок для создания операции.
        var initOperation = _fixture.Create<Operation>();
        _operationFacadeMock.Setup(o => o.Create(It.IsAny<OperationDto>())).Returns(initOperation);
    
        // Act: вызываем метод импорта.
        _facade.ImportBankAccountsFromFile(tempFile);
    
        // Assert: проверяем, что метод Create был вызван хотя бы один раз.
        _accountFacadeMock.Verify(a => a.Create(It.IsAny<BankAccountDto>()), Times.AtLeastOnce);
    
        File.Delete(tempFile);
    }

    [Fact]
    public void ExportBankAccountsFromFile_WritesFile_WithData()
    {
        // Arrange: используем тестовые аккаунты, которые наследуются от BankAccount и реализуют Accept
        var accounts = new List<BankAccount>
        {
            new TestBankAccount { Id = Guid.NewGuid(), Name = "Acc1", Balance = 100 },
            new TestBankAccount { Id = Guid.NewGuid(), Name = "Acc2", Balance = 200 }
        };
        _accountFacadeMock.Setup(a => a.GetAllBankAccounts()).Returns(accounts);

        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

        // Act
        _facade.ExportBankAccountsFromFile(tempFile);

        // Assert
        Assert.True(File.Exists(tempFile));
        string content = File.ReadAllText(tempFile);
        Assert.False(string.IsNullOrWhiteSpace(content));

        File.Delete(tempFile);
    }

    [Fact]
    public void ImportCategoriesFromFile_CallsCreateCategory_ForEachDTO()
    {
        // Arrange
        var categoryDto = _fixture.Create<CategoryDto>();
        var dtos = new List<CategoryDto> { categoryDto };
        string json = JsonSerializer.Serialize(dtos);
        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        File.WriteAllText(tempFile, json);

        var category = _fixture.Create<Category>();
        _categoryFacadeMock.Setup(c => c.Create(categoryDto)).Returns(category);

        // Act
        _facade.ImportCategoriesFromFile(tempFile);

        // Assert: проверяем, что CreateCategory был вызван
        _categoryFacadeMock.Verify(c => c.Create(It.IsAny<CategoryDto>()), Times.AtLeastOnce);

        File.Delete(tempFile);
    }

    [Fact]
    public void ExportCategoriesFromFile_WritesFile_WithData()
    {
        // Arrange
        var categories = _fixture.CreateMany<Category>(2).ToList();
        _categoryFacadeMock.Setup(c => c.GetAllCategories()).Returns(categories);

        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

        // Act
        _facade.ExportCategoriesFromFile(tempFile);

        // Assert
        Assert.True(File.Exists(tempFile));
        string content = File.ReadAllText(tempFile);
        Assert.False(string.IsNullOrWhiteSpace(content));

        File.Delete(tempFile);
    }

    [Fact]
    public void ImportOperationsFromFile_CallsCreateOperation_ForEachDTO()
    {
        // Arrange: создаём список DTO для операций
        var operationDto = _fixture.Create<OperationDto>();
        // Обеспечиваем, что сумма положительна, чтобы не было исключения в CreateOperation.
        operationDto.Amount = Math.Abs(operationDto.Amount);
    
        // Настраиваем моки:
        // Аккаунт должен существовать:
        _accountFacadeMock.Setup(a => a.AccountExists(operationDto.BankAccountId)).Returns(true);
        // Категория должна существовать:
        _categoryFacadeMock.Setup(c => c.CategoryExists(operationDto.CategoryId)).Returns(true);
        // Настраиваем GetById для категории, чтобы вернуть категорию с тем же CategoryId:
        var category = _fixture.Create<Category>();
        category.CategoryId = operationDto.CategoryId;
        _categoryFacadeMock.Setup(c => c.GetById(operationDto.CategoryId)).Returns(category);
    
        // Настраиваем мок для создания операции:
        var createdOperation = _fixture.Create<Operation>();
        _operationFacadeMock.Setup(o => o.Create(operationDto)).Returns(createdOperation);
    
        // Сериализуем список операций в JSON:
        var dtos = new List<OperationDto> { operationDto };
        string json = System.Text.Json.JsonSerializer.Serialize(dtos);
        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        File.WriteAllText(tempFile, json);

        // Act: вызываем импорт операций
        _facade.ImportOperationsFromFile(tempFile);

        // Assert: проверяем, что метод Create операции был вызван хотя бы один раз.
        _operationFacadeMock.Verify(o => o.Create(It.IsAny<OperationDto>()), Times.AtLeastOnce);

        File.Delete(tempFile);
    }

    [Fact]
    public void ExportOperationsFromFile_WritesFile_WithData()
    {
        // Arrange
        var operations = _fixture.CreateMany<Operation>(3).ToList();
        _operationFacadeMock.Setup(o => o.GetAllOperations()).Returns(operations);

        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

        // Act
        _facade.ExportOperationsFromFile(tempFile);

        // Assert
        Assert.True(File.Exists(tempFile));
        string content = File.ReadAllText(tempFile);
        Assert.False(string.IsNullOrWhiteSpace(content));

        File.Delete(tempFile);
    }


    [Fact]
    public void CreateOperation_ThrowsException_WhenAccountDoesNotExist()
    {
        // Arrange
        var operationDto = _fixture.Create<OperationDto>();
        _accountFacadeMock.Setup(a => a.AccountExists(operationDto.BankAccountId)).Returns(false);

        // Act & Assert: проверяем, что выбрасывается исключение с ожидаемым сообщением.
        var ex = Assert.Throws<ArgumentException>(() => _facade.CreateOperation(operationDto));
        Assert.Equal("Нет такого аккаунта BankAccountId", ex.Message);
    }

    [Fact]
    public void CreateOperation_ThrowsException_WhenCategoryDoesNotExist()
    {
        var operationDto = _fixture.Create<OperationDto>();
        _accountFacadeMock.Setup(a => a.AccountExists(operationDto.BankAccountId)).Returns(true);
        _categoryFacadeMock.Setup(c => c.CategoryExists(operationDto.CategoryId)).Returns(false);

        var ex = Assert.Throws<ArgumentException>(() => _facade.CreateOperation(operationDto));
        Assert.Contains("Нет такой категории", ex.Message);
        Assert.Contains("CategoryId", ex.Message);
    }

    [Fact]
    public void CreateOperation_ReturnsOperation_WhenValid()
    {
        var operationDto = _fixture.Create<OperationDto>();
        operationDto.Amount = 100;
        _accountFacadeMock.Setup(a => a.AccountExists(operationDto.BankAccountId)).Returns(true);
        _categoryFacadeMock.Setup(c => c.CategoryExists(operationDto.CategoryId)).Returns(true);
        var category = _fixture.Create<Category>();
        _categoryFacadeMock.Setup(c => c.GetById(operationDto.CategoryId)).Returns(category);
        var expectedOperation = _fixture.Create<Operation>();
        _operationFacadeMock.Setup(o => o.Create(operationDto)).Returns(expectedOperation);

        var result = _facade.CreateOperation(operationDto);

        Assert.Equal(expectedOperation, result);
        Assert.Equal(category.Type, operationDto.Type);
    }

    [Fact]
    public void EditOperation_ThrowsException_WhenCategoryDoesNotExist()
    {
        var editDto = _fixture.Create<EditOperationDto>();
        _categoryFacadeMock.Setup(c => c.CategoryExists(editDto.CategoryId)).Returns(false);

        var ex = Assert.Throws<ArgumentException>(() => _facade.EditOperation(editDto));
        Assert.Contains("Такой категории нет", ex.Message);
        Assert.Contains("CategoryId", ex.Message);
    }

    [Fact]
    public void EditOperation_ReturnsResult_FromOperationFacade()
    {
        var editDto = _fixture.Create<EditOperationDto>();
        _categoryFacadeMock.Setup(c => c.CategoryExists(editDto.CategoryId)).Returns(true);
        _operationFacadeMock.Setup(o => o.EditOperation(editDto)).Returns(true);

        var result = _facade.EditOperation(editDto);
        Assert.True(result);
    }

    [Fact]
    public void DeleteOperation_ReturnsResult_FromOperationFacade()
    {
        var opId = Guid.NewGuid();
        _operationFacadeMock.Setup(o => o.DeleteOperation(opId)).Returns(true);

        var result = _facade.DeleteOperation(opId);
        Assert.True(result);
    }

    [Fact]
    public void GetOperation_ReturnsOperation_FromOperationFacade()
    {
        var opId = Guid.NewGuid();
        var expectedOperation = _fixture.Create<Operation>();
        _operationFacadeMock.Setup(o => o.GetById(opId)).Returns(expectedOperation);

        var result = _facade.GetOperation(opId);
        Assert.Equal(expectedOperation, result);
    }
    
    [Fact]
    public void CreateBankAccount_UsesExistingCategory_WhenInitCategoryExists()
    {
        var bankAccountDto = _fixture.Create<BankAccountDto>();
        var account = _fixture.Create<BankAccount>();
        _accountFacadeMock.Setup(a => a.Create(bankAccountDto)).Returns(account);
        _accountFacadeMock.Setup(a => a.AccountExists(account.Id)).Returns(true);
    
        Guid initGuid = Guid.Parse(Constants.InitCategoryId);
        // Симулируем, что начальная категория существует
        _categoryFacadeMock.Setup(c => c.CategoryExists(initGuid)).Returns(true);
        var existingCategory = _fixture.Create<Category>();
        // Обязательно задаём идентификатор равным initGuid:
        existingCategory.CategoryId = initGuid;
        _categoryFacadeMock.Setup(c => c.GetById(initGuid)).Returns(existingCategory);
        _categoryFacadeMock.Setup(c => c.CategoryExists(existingCategory.CategoryId)).Returns(true);
    
        var initOperation = _fixture.Create<Operation>();
        _operationFacadeMock.Setup(o => o.Create(It.IsAny<OperationDto>())).Returns(initOperation);

        var result = _facade.CreateBankAccount(bankAccountDto);

        Assert.Equal(account, result);
        _accountFacadeMock.Verify(a => a.Create(bankAccountDto), Times.Once);
        // Ожидаем, что GetById будет вызван дважды, так как метод GetCategory вызывается дважды
        _categoryFacadeMock.Verify(c => c.GetById(initGuid), Times.Exactly(2));
        _operationFacadeMock.Verify(o => o.Create(It.Is<OperationDto>(dto =>
            dto.BankAccountId == account.Id &&
            dto.CategoryId == existingCategory.CategoryId &&
            dto.Description == Constants.InitOperationDescription &&
            dto.Type == Type.Income &&
            dto.Amount == bankAccountDto.Balance
        )), Times.Once);
    }
    
    [Fact]
    public void CreateBankAccount_CreatesCategory_WhenInitCategoryDoesNotExist()
    {
        var bankAccountDto = _fixture.Create<BankAccountDto>();
        var account = _fixture.Create<BankAccount>();
        _accountFacadeMock.Setup(a => a.Create(bankAccountDto)).Returns(account);
        _accountFacadeMock.Setup(a => a.AccountExists(account.Id)).Returns(true);
    
        Guid initGuid = Guid.Parse(Constants.InitCategoryId);
        // Симулируем, что начальная категория отсутствует
        _categoryFacadeMock.Setup(c => c.CategoryExists(initGuid)).Returns(false);
        var newCategory = _fixture.Create<Category>();
        _categoryFacadeMock.Setup(c => c.Create(It.Is<CategoryDto>(dto =>
                dto.Name == Constants.InitCategoryName &&
                dto.CategoryId == initGuid &&
                dto.Type == Type.Income)))
            .Returns(newCategory);
        // Настраиваем, чтобы после создания новой категории метод GetById возвращал эту категорию
        _categoryFacadeMock.Setup(c => c.GetById(newCategory.CategoryId)).Returns(newCategory);
        _categoryFacadeMock.Setup(c => c.CategoryExists(newCategory.CategoryId)).Returns(true);
    
        var initOperation = _fixture.Create<Operation>();
        _operationFacadeMock.Setup(o => o.Create(It.IsAny<OperationDto>())).Returns(initOperation);

        var result = _facade.CreateBankAccount(bankAccountDto);

        Assert.Equal(account, result);
        _accountFacadeMock.Verify(a => a.Create(bankAccountDto), Times.Once);
        _categoryFacadeMock.Verify(c => c.Create(It.IsAny<CategoryDto>()), Times.Once);
        _operationFacadeMock.Verify(o => o.Create(It.Is<OperationDto>(dto =>
            dto.BankAccountId == account.Id &&
            dto.CategoryId == newCategory.CategoryId &&
            dto.Description == Constants.InitOperationDescription &&
            dto.Type == Type.Income &&
            dto.Amount == bankAccountDto.Balance
        )), Times.Once);
    }

    [Fact]
    public void EditBankAccount_DelegatesToAccountFacade()
    {
        var editDto = _fixture.Create<EditBankAccountDto>();
        _accountFacadeMock.Setup(a => a.EditBankAccount(editDto)).Returns(true);

        var result = _facade.EditBankAccount(editDto);
        Assert.True(result);
    }

    [Fact]
    public void DeleteBankAccount_DelegatesToAccountFacade_AndDeletesAssociatedOperations()
    {
        Guid id = Guid.NewGuid();
        _accountFacadeMock.Setup(a => a.DeleteBankAccount(id)).Returns(true);
        var operations = new List<Operation>
        {
            new Operation { Id = Guid.NewGuid(), BankAccountId = id },
            new Operation { Id = Guid.NewGuid(), BankAccountId = id },
            new Operation { Id = Guid.NewGuid(), BankAccountId = Guid.NewGuid() } // unrelated
        };
        _operationFacadeMock.Setup(o => o.GetAllOperations()).Returns(operations);
        _operationFacadeMock.Setup(o => o.DeleteOperation(It.IsAny<Guid>())).Returns(true);

        var result = _facade.DeleteBankAccount(id);
        Assert.True(result);
        _accountFacadeMock.Verify(a => a.DeleteBankAccount(id), Times.Once);
        _operationFacadeMock.Verify(o => o.DeleteOperation(It.Is<Guid>(opId =>
            operations.Any(op => op.Id == opId && op.BankAccountId == id)
        )), Times.Exactly(2));
    }

    [Fact]
    public void GetBankAccount_DelegatesToAccountFacade()
    {
        Guid id = Guid.NewGuid();
        var account = _fixture.Create<BankAccount>();
        _accountFacadeMock.Setup(a => a.GetById(id)).Returns(account);

        var result = _facade.GetBankAccount(id);
        Assert.Equal(account, result);
    }

    [Fact]
    public void CreateCategory_DelegatesToCategoryFacade()
    {
        var categoryDto = _fixture.Create<CategoryDto>();
        var category = _fixture.Create<Category>();
        _categoryFacadeMock.Setup(c => c.Create(categoryDto)).Returns(category);

        var result = _facade.CreateCategory(categoryDto);
        Assert.Equal(category, result);
    }

    [Fact]
    public void EditCategory_DelegatesToCategoryFacade()
    {
        var editDto = _fixture.Create<EditCategoryDto>();
        _categoryFacadeMock.Setup(c => c.EditCategory(editDto)).Returns(true);

        var result = _facade.EditCategory(editDto);
        Assert.True(result);
    }

    [Fact]
    public void DeleteCategory_DelegatesToCategoryFacade_AndDeletesAssociatedOperations()
    {
        Guid id = Guid.NewGuid();
        _categoryFacadeMock.Setup(c => c.DeleteCategory(id)).Returns(true);
        var operations = new List<Operation>
        {
            new Operation { Id = Guid.NewGuid(), CategoryId = id },
            new Operation { Id = Guid.NewGuid(), CategoryId = id },
            new Operation { Id = Guid.NewGuid(), CategoryId = Guid.NewGuid() } // unrelated
        };
        _operationFacadeMock.Setup(o => o.GetAllOperations()).Returns(operations);
        _operationFacadeMock.Setup(o => o.DeleteOperation(It.IsAny<Guid>())).Returns(true);

        var result = _facade.DeleteCategory(id);
        Assert.True(result);
        _categoryFacadeMock.Verify(c => c.DeleteCategory(id), Times.Once);
        _operationFacadeMock.Verify(o => o.DeleteOperation(It.Is<Guid>(opId =>
            operations.Any(op => op.Id == opId && op.CategoryId == id)
        )), Times.Exactly(2));
    }

    [Fact]
    public void GetCategory_DelegatesToCategoryFacade()
    {
        Guid id = Guid.NewGuid();
        var category = _fixture.Create<Category>();
        _categoryFacadeMock.Setup(c => c.GetById(id)).Returns(category);

        var result = _facade.GetCategory(id);
        Assert.Equal(category, result);
    }

    [Fact]
    public void RecalculateBalance_ThrowsException_WhenAccountDoesNotExist()
    {
        Guid id = Guid.NewGuid();
        _accountFacadeMock.Setup(a => a.AccountExists(id)).Returns(false);

        var ex = Assert.Throws<ArgumentException>(() => _facade.RecalculateBalance(id));
        Assert.Contains(id.ToString(), ex.Message);
        Assert.Contains("не существует", ex.Message);
    }

    [Fact]
    public void RecalculateBalance_ReturnsCorrectBalance_AndUpdatesAccount()
    {
        Guid id = Guid.NewGuid();
        _accountFacadeMock.Setup(a => a.AccountExists(id)).Returns(true);
        var operations = new List<Operation>
        {
            new Operation { BankAccountId = id, Type = Type.Income, Amount = 100 },
            new Operation { BankAccountId = id, Type = Type.Expense, Amount = 50 },
            new Operation { BankAccountId = id, Type = Type.Income, Amount = 30 }
        };
        _operationFacadeMock.Setup(o => o.GetByCondition(It.IsAny<Func<Operation, bool>>()))
            .Returns(operations);
        var account = _fixture.Create<BankAccount>();
        _accountFacadeMock.Setup(a => a.GetById(id)).Returns(account);

        var result = _facade.RecalculateBalance(id);
        Assert.Equal(80, result);
        Assert.Equal(80, account.Balance);
    }

    [Fact]
    public void GetBalanceDifference_DelegatesToAnalyticsService()
    {
        var data = _fixture.Create<FinancialDataDto>();
        DateTime start = DateTime.Now.AddDays(-10);
        DateTime end = DateTime.Now;
        decimal expectedDiff = 123.45m;
        _analyticsServiceMock.Setup(a => a.GetBalanceDifference(data, start, end)).Returns(expectedDiff);

        var result = _facade.GetBalanceDifference(data, start, end);
        Assert.Equal(expectedDiff, result);
    }

    [Fact]
    public void GroupOperationsByCategory_DelegatesToAnalyticsService()
    {
        var data = _fixture.Create<FinancialDataDto>();
        var expectedDict = new Dictionary<Guid, List<Operation>>
        {
            { Guid.NewGuid(), _fixture.CreateMany<Operation>(2).ToList() }
        };
        _analyticsServiceMock.Setup(a => a.GroupOperationsByCategory(data)).Returns(expectedDict);

        var result = _facade.GroupOperationsByCategory(data);
        Assert.Equal(expectedDict, result);
    }

    [Fact]
    public void GetAllOperations_DelegatesToOperationFacade()
    {
        var operations = _fixture.CreateMany<Operation>(3);
        _operationFacadeMock.Setup(o => o.GetAllOperations()).Returns(operations);

        var result = _facade.GetAllOperations();
        Assert.Equal(operations, result);
    }

    [Fact]
    public void GetAllBankAccounts_DelegatesToAccountFacade()
    {
        var accounts = _fixture.CreateMany<BankAccount>(3);
        _accountFacadeMock.Setup(a => a.GetAllBankAccounts()).Returns(accounts);

        var result = _facade.GetAllBankAccounts();
        Assert.Equal(accounts, result);
    }

    [Fact]
    public void GetAllCategories_DelegatesToCategoryFacade()
    {
        var categories = _fixture.CreateMany<Category>(3);
        _categoryFacadeMock.Setup(c => c.GetAllCategories()).Returns(categories);

        var result = _facade.GetAllCategories();
        Assert.Equal(categories, result);
    }
}