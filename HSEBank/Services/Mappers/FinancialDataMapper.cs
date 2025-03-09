using HSEBank.Dto;
using HSEBank.Models;

namespace HSEBank.Services.Mappers;

public static class FinancialDataMapper
{
    public static FinancialDataDto Map(
        IEnumerable<BankAccount> bankAccounts,
        IEnumerable<Category> categories,
        IEnumerable<Operation> operations)
    {
        return new FinancialDataDto
        {
            BankAccounts = bankAccounts.ToList(),
            Categories = categories.ToList(),
            Operations = operations.ToList()
        };
    }
}