using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.DataAccess.Common.Enums;
using HSEBank.DataAccess.Models;
using HSEBank.Presentation.Common;
using Type = HSEBank.DataAccess.Common.Enums.Type;

namespace HSEBank.Presentation;

public class FinancialFacade : IFinancialFacade
{
    private static FinancialFacade _instance;
    
    private readonly IAccountFacade _accountFacade;
    private readonly ICategoryFacade _categoryFacade;
    private readonly IOperationFacade _operationFacade;
    private readonly IAnalyticsService _analyticsService;

    private FinancialFacade(
        IAccountFacade accountFacade,
        ICategoryFacade categoryFacade,
        IOperationFacade operationFacade,
        IAnalyticsService analyticsService)
    {
        _accountFacade = accountFacade;
        _categoryFacade = categoryFacade;
        _operationFacade = operationFacade;
        _analyticsService = analyticsService;
    }

    public static FinancialFacade GetInstance(IAccountFacade accountFacade,
        ICategoryFacade categoryFacade,
        IOperationFacade operationFacade,
        IAnalyticsService analyticsService)
    {
        if(_instance == null)
        {
            _instance = new FinancialFacade(accountFacade, categoryFacade, operationFacade, analyticsService);
        }
        return _instance;
    }
    
    public Operation CreateOperation(OperationDto operationDto)
    {
        if (!_accountFacade.AccountExists(operationDto.BankAccountId))
        {
            throw new ArgumentException($"Account does not exist {nameof(operationDto.BankAccountId)}");
        }
        
        if (!_categoryFacade.CategoryExists(operationDto.CategoryId))
        {
            throw new ArgumentException($"Category does not exist {nameof(operationDto.CategoryId)}");
        }
        
        var category = GetCategory(operationDto.CategoryId);

        operationDto.Type = category.Type;
        
        return _operationFacade.Create(operationDto);
    }

    public bool EditOperation(EditOperationDto editOperationDto)
    {
        if (!_categoryFacade.CategoryExists(editOperationDto.CategoryId))
        {
            throw new ArgumentException($"Category does not exist {nameof(editOperationDto.CategoryId)}");
        }
        return _operationFacade.EditOperation(editOperationDto);
    }

    public bool DeleteOperation(Guid operationId)
    {
        return _operationFacade.DeleteOperation(operationId);
    }

    public Operation GetOperation(Guid operationId)
    {
        return _operationFacade.GetById(operationId);
    }

    public BankAccount CreateBankAccount(BankAccountDto bankAccountDto)
    {
        var account =  _accountFacade.Create(bankAccountDto);

        Guid categoryId;

        if (!_categoryFacade.CategoryExists(Guid.Parse(Constants.InitCategoryGuid)))
        {
            var category = CreateCategory(new CategoryDto()
            {
                Name = Constants.InitCategoryName,
                CategoryId = Guid.Parse(Constants.InitCategoryGuid),
                Type = Type.Income
            });
            
            categoryId = category.Id;
        }
        else
        {
            categoryId = GetCategory(Guid.Parse(Constants.InitCategoryGuid)).Id;
        }

        var operation = new OperationDto()
        {
            Amount = 0,
            BankAccountId = account.Id,
            CategoryId = categoryId,
            Description = "Initial bank account operation",
            Type = Type.Income
        };
        
        CreateOperation(operation);
        
        return account;
    }

    public bool EditBankAccount(EditBankAccountDto editBankAccountDto)
    {
        return _accountFacade.EditBankAccount(editBankAccountDto);
    }

    public bool DeleteBankAccount(Guid bankAccountId)
    {
        return _accountFacade.DeleteBankAccount(bankAccountId);
    }

    public BankAccount GetBankAccount(Guid bankAccountId)
    {
        return _accountFacade.GetById(bankAccountId);
    }

    public Category CreateCategory(CategoryDto categoryDto)
    {
        return _categoryFacade.Create(categoryDto);
    }

    public bool EditCategory(EditCategoryDto editCategoryDto)
    {
        return _categoryFacade.EditCategory(editCategoryDto);
    }

    public bool DeleteCategory(Guid categoryId)
    {
        return _categoryFacade.DeleteCategory(categoryId);
    }

    public Category GetCategory(Guid categoryId)
    {
        return _categoryFacade.GetById(categoryId);
    }

    public decimal RecalculateBalance(Guid bankAccountId)
    {
        if (!_accountFacade.AccountExists(bankAccountId))
        {
            throw new ArgumentException($"Account does not exist {nameof(bankAccountId)}");
        }
        decimal balance = 0;
        foreach (var op in _operationFacade.GetByCondition(o => o.BankAccountId == bankAccountId))
        {
            balance += (op.Type == Type.Income ? op.Amount : -op.Amount);
        }
        return balance;
    }

    public void ImportAccountsFromJson(string filePath)
    {
        var jsonImporter = new JsonDataImporter<BankAccountDto>();
        
        var accountDtos = jsonImporter.Import(filePath);

        foreach (var account in accountDtos)
        {
            CreateBankAccount(account);
        }
    }

    public void ExportAccountsFromJson(string filePath)
    {
        var exportVisitor = new JsonAggregateExportVisitor();

        var accounts = GetAllBankAccounts().ToList();

        foreach (var account in accounts)
        {
            account.Accept(exportVisitor);
        }
        
        exportVisitor.SaveToFile(filePath);
    }

    public decimal GetBalanceDifference(FinancialDataDto data, DateTime start, DateTime end)
    {
        return _analyticsService.GetBalanceDifference(data, start, end);
    }

    public Dictionary<Guid, List<Operation>> GroupOperationsByCategory(FinancialDataDto data)
    {
        return _analyticsService.GroupOperationsByCategory(data);
    }

    public IEnumerable<Operation> GetAllOperations() => _operationFacade.GetAllOperations();
    public IEnumerable<BankAccount> GetAllBankAccounts() => _accountFacade.GetAllBankAccounts();
    public IEnumerable<Category> GetAllCategories() => _categoryFacade.GetAllCategories();
}