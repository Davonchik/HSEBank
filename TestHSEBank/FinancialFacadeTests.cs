using System.Reflection;
using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.DataAccess.Models;
using HSEBank.Presentation;
using Moq;
using Type = HSEBank.DataAccess.Common.Enums.Type;

namespace TestHSEBank;

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
        // Сброс синглтона для независимости тестов.
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
    public void CreateOperation_Throws_Exception_When_Account_DoesNotExist()
    {
        var operationDto = _fixture.Create<OperationDto>();
        _accountFacadeMock.Setup(a => a.AccountExists(operationDto.BankAccountId)).Returns(false);
        
        var ex = Assert.Throws<ArgumentException>(() => _facade.CreateOperation(operationDto));
        Assert.Equal($"Account does not exist {nameof(operationDto.BankAccountId)}", ex.Message);
    }
    
    [Fact]
    public void CreateOperation_Throws_Exception_When_Category_DoesNotExist()
    {
        var operationDto = _fixture.Create<OperationDto>();
        _accountFacadeMock.Setup(a => a.AccountExists(operationDto.BankAccountId)).Returns(true);
        _categoryFacadeMock.Setup(c => c.CategoryExists(operationDto.CategoryId)).Returns(false);
        
        var ex = Assert.Throws<ArgumentException>(() => _facade.CreateOperation(operationDto));
        Assert.Equal($"Category does not exist {nameof(operationDto.CategoryId)}", ex.Message);
    }
    
    [Fact]
    public void CreateOperation_Sets_Type_From_Category_And_Calls_Create()
    {
        var operationDto = _fixture.Create<OperationDto>();
        var category = _fixture.Create<Category>();
        _accountFacadeMock.Setup(a => a.AccountExists(operationDto.BankAccountId)).Returns(true);
        _categoryFacadeMock.Setup(c => c.CategoryExists(operationDto.CategoryId)).Returns(true);
        _categoryFacadeMock.Setup(c => c.GetById(operationDto.CategoryId)).Returns(category);
        var expectedOperation = _fixture.Create<Operation>();
        _operationFacadeMock.Setup(o => o.Create(operationDto)).Returns(expectedOperation);
        
        var result = _facade.CreateOperation(operationDto);
        
        // Проверяем, что тип операции устанавливается из категории
        Assert.Equal(category.Type, operationDto.Type);
        Assert.Equal(expectedOperation, result);
    }
    
    [Fact]
    public void EditOperation_Throws_Exception_When_Category_DoesNotExist()
    {
        var editDto = _fixture.Create<EditOperationDto>();
        _categoryFacadeMock.Setup(c => c.CategoryExists(editDto.CategoryId)).Returns(false);
        
        var ex = Assert.Throws<ArgumentException>(() => _facade.EditOperation(editDto));
        Assert.Equal($"Category does not exist {nameof(editDto.CategoryId)}", ex.Message);
    }
    
    [Fact]
    public void EditOperation_Returns_True_When_Update_Succeeds()
    {
        var editDto = _fixture.Create<EditOperationDto>();
        _categoryFacadeMock.Setup(c => c.CategoryExists(editDto.CategoryId)).Returns(true);
        _operationFacadeMock.Setup(o => o.EditOperation(editDto)).Returns(true);
        
        var result = _facade.EditOperation(editDto);
        Assert.True(result);
    }
    
    [Fact]
    public void DeleteOperation_Returns_Result_From_OperationFacade()
    {
        Guid opId = Guid.NewGuid();
        _operationFacadeMock.Setup(o => o.DeleteOperation(opId)).Returns(true);
        
        var result = _facade.DeleteOperation(opId);
        Assert.True(result);
    }
    
    [Fact]
    public void GetOperation_Returns_Operation_When_It_Exists()
    {
        Guid opId = Guid.NewGuid();
        var expectedOperation = _fixture.Create<Operation>();
        _operationFacadeMock.Setup(o => o.GetById(opId)).Returns(expectedOperation);
        
        var result = _facade.GetOperation(opId);
        Assert.Equal(expectedOperation, result);
    }
    
    [Fact]
    public void EditBankAccount_Delegates_To_AccountFacade()
    {
        var editDto = _fixture.Create<EditBankAccountDto>();
        _accountFacadeMock.Setup(a => a.EditBankAccount(editDto)).Returns(true);
        
        var result = _facade.EditBankAccount(editDto);
        Assert.True(result);
    }
    
    [Fact]
    public void DeleteBankAccount_Delegates_To_AccountFacade()
    {
        Guid id = Guid.NewGuid();
        _accountFacadeMock.Setup(a => a.DeleteBankAccount(id)).Returns(true);
        
        var result = _facade.DeleteBankAccount(id);
        Assert.True(result);
    }
    
    [Fact]
    public void GetBankAccount_Delegates_To_AccountFacade()
    {
        Guid id = Guid.NewGuid();
        var account = _fixture.Create<BankAccount>();
        _accountFacadeMock.Setup(a => a.GetById(id)).Returns(account);
        
        var result = _facade.GetBankAccount(id);
        Assert.Equal(account, result);
    }
    
    [Fact]
    public void CreateCategory_Delegates_To_CategoryFacade()
    {
        var categoryDto = _fixture.Create<CategoryDto>();
        var category = _fixture.Create<Category>();
        _categoryFacadeMock.Setup(c => c.Create(categoryDto)).Returns(category);
        
        var result = _facade.CreateCategory(categoryDto);
        Assert.Equal(category, result);
    }
    
    [Fact]
    public void EditCategory_Delegates_To_CategoryFacade()
    {
        var editDto = _fixture.Create<EditCategoryDto>();
        _categoryFacadeMock.Setup(c => c.EditCategory(editDto)).Returns(true);
        
        var result = _facade.EditCategory(editDto);
        Assert.True(result);
    }
    
    [Fact]
    public void DeleteCategory_Delegates_To_CategoryFacade()
    {
        Guid id = Guid.NewGuid();
        _categoryFacadeMock.Setup(c => c.DeleteCategory(id)).Returns(true);
        
        var result = _facade.DeleteCategory(id);
        Assert.True(result);
    }
    
    [Fact]
    public void GetCategory_Delegates_To_CategoryFacade()
    {
        Guid id = Guid.NewGuid();
        var category = _fixture.Create<Category>();
        _categoryFacadeMock.Setup(c => c.GetById(id)).Returns(category);
        
        var result = _facade.GetCategory(id);
        Assert.Equal(category, result);
    }
    
    [Fact]
    public void RecalculateBalance_Throws_Exception_When_Account_DoesNotExist()
    {
        Guid bankAccountId = Guid.NewGuid();
        _accountFacadeMock.Setup(a => a.AccountExists(bankAccountId)).Returns(false);
    
        var ex = Assert.Throws<ArgumentException>(() => _facade.RecalculateBalance(bankAccountId));
        // Обратите внимание, что nameof(bankAccountId) возвращает "bankAccountId"
        Assert.Equal($"Account does not exist bankAccountId", ex.Message);
    }
    
    [Fact]
    public void RecalculateBalance_Returns_Correct_Balance()
    {
        Guid id = Guid.NewGuid();
        _accountFacadeMock.Setup(a => a.AccountExists(id)).Returns(true);
        // Создаем операции: доход 100, расход 50, доход 30 => баланс 80
        var operations = new List<Operation>
        {
            new Operation { BankAccountId = id, Type = Type.Income, Amount = 100 },
            new Operation { BankAccountId = id, Type = Type.Expense, Amount = 50 },
            new Operation { BankAccountId = id, Type = Type.Income, Amount = 30 }
        };
        _operationFacadeMock.Setup(o => o.GetByCondition(It.IsAny<Func<Operation, bool>>()))
            .Returns(operations);
        
        var result = _facade.RecalculateBalance(id);
        Assert.Equal(80, result);
    }
    
    // [Fact]
    // public void ExportAccountsFromJson_Creates_File_With_Exported_Data()
    // {
    //     // Настраиваем GetAllBankAccounts для возврата списка аккаунтов
    //     var accounts = _fixture.CreateMany<BankAccount>(2).ToList();
    //     // Для теста зададим простую реализацию метода Accept через делегат (если Account.Accept является виртуальным или публичным)
    //     foreach (var account in accounts)
    //     {
    //         account.Accept = visitor => { /* ничего не делаем для теста */ };
    //     }
    //     _accountFacadeMock.Setup(a => a.GetAllBankAccounts()).Returns(accounts);
    //     
    //     var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
    //     
    //     _facade.ExportAccountsFromJson(tempFile);
    //     
    //     Assert.True(File.Exists(tempFile));
    //     var content = File.ReadAllText(tempFile);
    //     Assert.False(string.IsNullOrWhiteSpace(content));
    //     
    //     File.Delete(tempFile);
    // }
    
    [Fact]
    public void GetBalanceDifference_Returns_Value_From_AnalyticsService()
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
    public void GroupOperationsByCategory_Returns_Dictionary_From_AnalyticsService()
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
    public void GetAllOperations_Delegates_To_OperationFacade()
    {
        var operations = _fixture.CreateMany<Operation>(3);
        _operationFacadeMock.Setup(o => o.GetAllOperations()).Returns(operations);
        
        var result = _facade.GetAllOperations();
        Assert.Equal(operations, result);
    }
    
    [Fact]
    public void GetAllBankAccounts_Delegates_To_AccountFacade()
    {
        var accounts = _fixture.CreateMany<BankAccount>(3);
        _accountFacadeMock.Setup(a => a.GetAllBankAccounts()).Returns(accounts);
        
        var result = _facade.GetAllBankAccounts();
        Assert.Equal(accounts, result);
    }
    
    [Fact]
    public void GetAllCategories_Delegates_To_CategoryFacade()
    {
        var categories = _fixture.CreateMany<Category>(3);
        _categoryFacadeMock.Setup(c => c.GetAllCategories()).Returns(categories);
        
        var result = _facade.GetAllCategories();
        Assert.Equal(categories, result);
    }
}