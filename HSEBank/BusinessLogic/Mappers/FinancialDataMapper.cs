using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Mappers;

/// <summary>
/// For data mapping.
/// </summary>
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