namespace HSEBank.BusinessLogic.Dto;

/// <summary>
/// Dto for creating bank account.
/// </summary>
public class BankAccountDto
{
    public Guid AccountId { get; set; }
    public string Name { get; set; }
    public decimal InitBalance { get; set; }
}