using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Dto;

/// <summary>
/// Financial Data Dto.
/// </summary>
public class FinancialDataDto
{
    public List<BankAccount> BankAccounts { get; set; }
    public List<Category> Categories { get; set; }
    public List<Operation> Operations { get; set; }
}