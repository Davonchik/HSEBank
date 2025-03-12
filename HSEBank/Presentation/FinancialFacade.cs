using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.DataAccess.Models;

namespace HSEBank.Presentation;

public class FinancialFacade : IFinancialFacade
{
    private static FinancialFacade _instance;
    
    private readonly IAccountFacade _accountFacade;
    private readonly ICategoryFacade _categoryFacade;
    private readonly IOperationFacade _operationFacade;
    // private readonly IFinancialFacade _financialFacade;

    private FinancialFacade(
        IAccountFacade accountFacade,
        ICategoryFacade categoryFacade,
        IOperationFacade operationFacade)
    {
        _accountFacade = accountFacade;
        _categoryFacade = categoryFacade;
        _operationFacade = operationFacade;
        // _financialFacade = financialFacade;
    }

    public static FinancialFacade GetInstance(IAccountFacade accountFacade,
        ICategoryFacade categoryFacade,
        IOperationFacade operationFacade)
    {
        if(_instance == null)
        {
            _instance = new FinancialFacade(accountFacade, categoryFacade, operationFacade);
        }
        return _instance;
    }
    
    public Operation CreateOperation(OperationDto operationDto)
    {
        // сделать ли проверку, что есть уже такая операция ?
        if (!_accountFacade.AccountExists(operationDto.BankAccountId))
        {
            throw new ArgumentException($"Account does not exist {nameof(operationDto.BankAccountId)}");
        }
        
        if (!_categoryFacade.CategoryExists(operationDto.CategoryId))
        {
            throw new ArgumentException($"Category does not exist {nameof(operationDto.CategoryId)}");
        }
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
        if (!_operationFacade.OperationExists(operationId))
        {
            throw new ArgumentException($"Operation does not exist {nameof(operationId)}");
        }
        return _operationFacade.DeleteOperation(operationId);
    }

    public Operation GetOperation(Guid operationId)
    {
        if (!_operationFacade.OperationExists(operationId))
        {
            throw new ArgumentException($"Operation does not exist {nameof(operationId)}");
        }
        return _operationFacade.GetById(operationId);
    }

    public BankAccount CreateBankAccount(BankAccountDto bankAccountDto)
    {
        if (_accountFacade.AccountExists(bankAccountDto.AccountId))
        {
            throw new ArgumentException($"Account is already exist {nameof(bankAccountDto.AccountId)}");
        }
        return _accountFacade.Create(bankAccountDto);
    }

    public bool EditBankAccount(EditBankAccountDto editBankAccountDto)
    {
        if (!_accountFacade.AccountExists(editBankAccountDto.BankAccountId))
        {
            throw new ArgumentException($"Account does not exist {nameof(editBankAccountDto.BankAccountId)}");
        }
        return _accountFacade.EditBankAccount(editBankAccountDto);
    }

    public bool DeleteBankAccount(Guid bankAccountId)
    {
        if (!_accountFacade.AccountExists(bankAccountId))
        {
            throw new ArgumentException($"Account does not exist {nameof(bankAccountId)}");
        }
        return _accountFacade.DeleteBankAccount(bankAccountId);
    }

    public BankAccount GetBankAccount(Guid bankAccountId)
    {
        if (!_accountFacade.AccountExists(bankAccountId))
        {
            throw new ArgumentException($"Account does not exist {nameof(bankAccountId)}");
        }
        return _accountFacade.GetById(bankAccountId);
    }

    public Category CreateCategory(CategoryDto categoryDto)
    {
        if (_categoryFacade.CategoryExists(categoryDto.CategoryId))
        {
            throw new ArgumentException($"Category is already exist {nameof(categoryDto.CategoryId)}");
        }
        return _categoryFacade.Create(categoryDto);
    }

    public bool EditCategory(EditCategoryDto editCategoryDto)
    {
        return _categoryFacade.EditCategory(editCategoryDto);
    }

    public bool DeleteCategory(Guid categoryId)
    {
        if (!_categoryFacade.CategoryExists(categoryId))
        {
            throw new ArgumentException($"Category does not exist {nameof(categoryId)}");
        }
        return _categoryFacade.DeleteCategory(categoryId);
    }

    public Category GetCategory(Guid categoryId)
    {
        if (!_categoryFacade.CategoryExists(categoryId))
        {
            throw new ArgumentException($"Category does not exist {nameof(categoryId)}");
        }
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
            balance += (op.Type == BusinessLogic.Shared.OperationType.Income ? op.Amount : -op.Amount);
        }
        return balance;
    }

    public IEnumerable<Operation> GetAllOperations() => _operationFacade.GetAllOperations();
    public IEnumerable<BankAccount> GetAllBankAccounts() => _accountFacade.GetAllBankAccounts();
    public IEnumerable<Category> GetAllCategories() => _categoryFacade.GetAllCategories();
}