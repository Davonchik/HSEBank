using HSEBank.DataAccess.Common.Enums;
using Type = HSEBank.DataAccess.Common.Enums.Type;

namespace HSEBank.BusinessLogic.Dto;

/// <summary>
/// Dto for category creating.
/// </summary>
public class CategoryDto
{
    public Guid? CategoryId { get; set; }
    public Type Type { get; set; }
    public string Name { get; set; }
}