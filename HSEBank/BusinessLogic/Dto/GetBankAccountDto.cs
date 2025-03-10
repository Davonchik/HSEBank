namespace HSEBank.BusinessLogic.Dto;

/// <summary>
/// Dto for getting bank account information.
/// </summary>
public class GetBankAccountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
}