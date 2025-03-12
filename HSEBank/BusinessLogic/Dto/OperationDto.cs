using Type = HSEBank.DataAccess.Common.Enums.Type;

namespace HSEBank.BusinessLogic.Dto;

/// <summary>
/// Dto for operation creating.
/// </summary>
public class OperationDto
{
    public Type Type { get; set; }
    public Guid BankAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
}