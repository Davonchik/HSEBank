using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Services.Abstractions;

/// <summary>
/// Facade for combining operations on transactions, accounts and categories.
/// </summary>
public interface IFinancialFacade
{
    Operation CreateOperation(OperationDto operationDto);
    bool EditOperation(EditOperationDto editOperationDto);
    bool DeleteOperation(Guid operationId);
    Operation GetOperation(Guid operationId);
    
    BankAccount CreateBankAccount(BankAccountDto bankAccountDto);
    bool EditBankAccount(EditBankAccountDto editBankAccountDto);
    bool DeleteBankAccount(Guid bankAccountId);
    BankAccount GetBankAccount(Guid bankAccountId);
    
    Category CreateCategory(CategoryDto categoryDto);
    bool EditCategory(EditCategoryDto editCategoryDto);
    bool DeleteCategory(Guid categoryId);
    Category GetCategory(Guid categoryId);
    
    decimal RecalculateBalance(Guid bankAccountId);
    
    IEnumerable<Operation> GetAllOperations();
    IEnumerable<BankAccount> GetAllBankAccounts();
    IEnumerable<Category> GetAllCategories();
}