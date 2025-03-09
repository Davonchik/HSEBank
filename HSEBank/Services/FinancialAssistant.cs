using HSEBank.Abstractions;
using HSEBank.Dto;
using HSEBank.Models;

namespace HSEBank.Services;

public class FinancialAssistant : IFinancialAssistant
{
    private readonly IAccountService _accountService;
    private readonly ICategoryService _categoryService;
    private readonly IOperationService _operationService;
    private readonly IFinancialFactory _financialFactory;

    public FinancialAssistant(
        IAccountService accountService,
        ICategoryService categoryService,
        IOperationService operationService,
        IFinancialFactory financialFactory)
    {
        _accountService = accountService;
        _categoryService = categoryService;
        _operationService = operationService;
        _financialFactory = financialFactory;
    }
    
    public Operation CreateOperation(OperationDto operationDto)
    {
        return _operationService.Create(operationDto);
    }

    public bool EditOperation(EditOperationDto editOperationDto)
    {
        return _operationService.Update(editOperationDto);
    }

    public bool DeleteOperation(Guid operationId)
    {
        return _operationService.Delete(operationId);
    }

    public Operation GetOperation(Guid operationId)
    {
        return _operationService.GetById(operationId);
    }

    public BankAccount CreateBankAccount(BankAccountDto bankAccountDto)
    {
        return _accountService.Create(bankAccountDto);
    }

    public bool EditBankAccount(EditBankAccountDto editBankAccountDto)
    {
        return _accountService.Update(editBankAccountDto);
    }

    public bool DeleteBankAccount(Guid bankAccountId)
    {
        return _accountService.Delete(bankAccountId);
    }

    public BankAccount GetBankAccount(Guid bankAccountId)
    {
        return _accountService.GetById(bankAccountId);
    }

    public Category CreateCategory(CategoryDto categoryDto)
    {
        return _categoryService.Create(categoryDto);
    }

    public bool EditCategory(EditCategoryDto editCategoryDto)
    {
        return _categoryService.Update(editCategoryDto);
    }

    public bool DeleteCategory(Guid categoryId)
    {
        return _categoryService.Delete(categoryId);
    }

    public Category GetCategory(Guid categoryId)
    {
        return _categoryService.GetById(categoryId);
    }

    public decimal RecalculateBalance(Guid bankAccountId)
    {
        decimal balance = 0;
        foreach (var op in _operationService.GetByCondition(o => o.BankAccountId == bankAccountId))
        {
            balance += (op.Type == Shared.OperationType.Income ? op.Amount : -op.Amount);
        }
        return balance;
    }

    public IEnumerable<Operation> GetAllOperations() => _operationService.GetAll();
    public IEnumerable<BankAccount> GetAllBankAccounts() => _accountService.GetAll();
    public IEnumerable<Category> GetAllCategories() => _categoryService.GetAll();
}