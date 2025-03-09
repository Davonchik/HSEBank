using HSEBank.Models;

namespace HSEBank.Dto;

public class FinancialDataDto
{
    public List<BankAccount> BankAccounts { get; set; }
    public List<Category> Categories { get; set; }
    public List<Operation> Operations { get; set; }
}