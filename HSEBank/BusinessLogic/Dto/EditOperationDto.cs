namespace HSEBank.BusinessLogic.Dto;

/// <summary>
/// Dto for operation editing.
/// </summary>
public class EditOperationDto
{
    public Guid OperationId { get; set; }
    public Guid CategoryId { get; set; }
}