using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.DataAccess.Common.Enums;
using HSEBank.DataAccess.Models;
using YamlDotNet.Core;
using Constants = HSEBank.DataAccess.Common.Constants.Constants;
using Type = HSEBank.DataAccess.Common.Enums.Type;

namespace HSEBank.Presentation;

/// <summary>
/// Main service (Facade) for user actions with HSE-Bank-Service.
/// </summary>
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
            throw new ArgumentException($"Нет такого аккаунта {nameof(operationDto.BankAccountId)}");
        }
        
        if (!_categoryFacade.CategoryExists(operationDto.CategoryId))
        {
            throw new ArgumentException($"Нет такой категории {nameof(operationDto.CategoryId)}");
        }
        
        var category = GetCategory(operationDto.CategoryId);

        operationDto.Type = category.Type;
        
        return _operationFacade.Create(operationDto);
    }

    public bool EditOperation(EditOperationDto editOperationDto)
    {
        if (!_categoryFacade.CategoryExists(editOperationDto.CategoryId))
        {
            throw new ArgumentException($"Такой категории нет {nameof(editOperationDto.CategoryId)}");
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
        var createdAccount = _accountFacade.Create(bankAccountDto);
        
        Guid categoryId;
        
        if (!_categoryFacade.CategoryExists(Guid.Parse(Constants.InitCategoryId)))
        {
            var category = CreateCategory(new CategoryDto
            {
                Name = Constants.InitCategoryName,
                CategoryId = Guid.Parse(Constants.InitCategoryId),
                Type = Type.Income
            });

            categoryId = category.CategoryId;
        }
        else
        {
            categoryId = GetCategory(Guid.Parse(Constants.InitCategoryId)).CategoryId;
        }

        var operation = new OperationDto
        {
            Amount = bankAccountDto.Balance,
            CategoryId = categoryId,
            Description = Constants.InitOperationDescription,
            BankAccountId = createdAccount.Id,
            Type = Type.Income
        };
        
        CreateOperation(operation);
        
        return createdAccount;
    }

    public bool EditBankAccount(EditBankAccountDto editBankAccountDto)
    {
        return _accountFacade.EditBankAccount(editBankAccountDto);
    }

    public bool DeleteBankAccount(Guid bankAccountId)
    {
        var del = _accountFacade.DeleteBankAccount(bankAccountId);

        if (del)
        {
            var operations = _operationFacade.GetAllOperations();
            foreach (var op in operations)
            {
                if (op.BankAccountId == bankAccountId)
                {
                    _operationFacade.DeleteOperation(op.Id);
                }
            }
        }
        
        return del;
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
        var del = _categoryFacade.DeleteCategory(categoryId);

        if (del)
        {
            var operations = _operationFacade.GetAllOperations();
            foreach (var op in operations)
            {
                if (op.CategoryId == categoryId)
                {
                    _operationFacade.DeleteOperation(op.Id);
                }
            }
        }
        
        return del;
    }

    public Category GetCategory(Guid categoryId)
    {
        return _categoryFacade.GetById(categoryId);
    }

    public decimal RecalculateBalance(Guid bankAccountId)
    {
        if (!_accountFacade.AccountExists(bankAccountId))
        {
            throw new ArgumentException($"Аккаунта с таким ID'{bankAccountId}' не существует!");
        }
        
        decimal balance = 0;
        
        foreach (var op in _operationFacade.GetByCondition(o => o.BankAccountId == bankAccountId))
        {
            balance += (op.Type == Type.Income ? op.Amount : -op.Amount);
        }

        _accountFacade.GetById(bankAccountId).Balance = balance;
        
        return balance;
    }
    
    public void ImportBankAccountsFromFile(string filePath)
    {
        var importer = DataTransferFactory.CreateImporter<BankAccountDto>(filePath);

        var accountsDtos = importer.Import(filePath);

        foreach (var account in accountsDtos)
        {
            CreateBankAccount(account);
        }
    }

    public void ExportBankAccountsFromFile(string filePath)
    {
        var exportVisitor = DataTransferFactory.CreateExporter(filePath);
        
        var accounts = GetAllBankAccounts().ToList();

        foreach (var account in accounts)
        {
            account.Accept(exportVisitor);
        }
        
        exportVisitor.SaveToFile(filePath);
    }

    public void ImportCategoriesFromFile(string filePath)
    {
        var importer = DataTransferFactory.CreateImporter<CategoryDto>(filePath);
        
        var categories = importer.Import(filePath);

        foreach (var category in categories)
        {
            CreateCategory(category);
        }
    }

    public void ExportCategoriesFromFile(string filePath)
    {
        var exportVisitor = DataTransferFactory.CreateExporter(filePath);
        
        var categories = GetAllCategories().ToList();

        foreach (var category in categories)
        {
            category.Accept(exportVisitor);
        }
        
        exportVisitor.SaveToFile(filePath);
    }
    
    public void ImportOperationsFromFile(string filePath)
    {
        var importer = DataTransferFactory.CreateImporter<OperationDto>(filePath);
        
        var operationDtos = importer.Import(filePath);

        foreach (var op in operationDtos)
        {
            CreateOperation(op);
        }
    }

    public void ExportOperationsFromFile(string filePath)
    {
        var exportVisitor = DataTransferFactory.CreateExporter(filePath);

        var operations = GetAllOperations().ToList();

        foreach (var op in operations)
        {
            op.Accept(exportVisitor);
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