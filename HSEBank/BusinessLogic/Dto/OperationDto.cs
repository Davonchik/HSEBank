using HSEBank.BusinessLogic.Shared;

namespace HSEBank.BusinessLogic.Dto;

/// <summary>
/// Dto for operation creating.
/// </summary>
public class OperationDto
{
    public OperationType Type { get; set; }
    public Guid BankAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
}