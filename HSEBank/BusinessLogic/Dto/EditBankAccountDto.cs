namespace HSEBank.BusinessLogic.Dto;

/// <summary>
/// Dto for editing bank account.
/// </summary>
public class EditBankAccountDto
{
    public Guid BankAccountId { get; set; }
    public string Name { get; set; }
}