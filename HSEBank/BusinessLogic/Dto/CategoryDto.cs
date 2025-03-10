using HSEBank.DataAccess.Common.Enums;

namespace HSEBank.BusinessLogic.Dto;

/// <summary>
/// Dto for category creating.
/// </summary>
public class CategoryDto
{
    public CategoryType Type { get; set; }
    public string Name { get; set; }
}