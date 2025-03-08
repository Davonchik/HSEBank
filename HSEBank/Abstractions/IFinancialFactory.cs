using HSEBank.Dto;
using HSEBank.Models;

namespace HSEBank.Abstractions;

/// <summary>
/// Factory for centralized creation of domain objects with validation.
/// </summary>
public interface IFinancialFactory
{
    Operation CreateOperation(OperationDto operationDto);
    BankAccount CreateBankAccount(BankAccountDto bankAccountDto);
    Category CreateCategory(CategoryDto categoryDto);
    //TODO по аналогии сделать создание категорий и бансковских счетов.
}