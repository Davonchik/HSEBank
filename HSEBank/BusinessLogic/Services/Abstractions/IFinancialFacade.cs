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

    public void ImportAccountsFromJson(string filePath);

    public void ExportAccountsFromJson(string filePath);

    public void ImportOperationsFromFile(string filePath);

    public void ExportOperationsFromFile(string filePath);

    public decimal GetBalanceDifference(FinancialDataDto data, DateTime start, DateTime end);

    public Dictionary<Guid, List<Operation>> GroupOperationsByCategory(FinancialDataDto data);
}