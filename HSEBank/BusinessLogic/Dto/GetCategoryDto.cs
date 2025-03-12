using HSEBank.DataAccess.Common.Enums;
using Type = HSEBank.DataAccess.Common.Enums.Type;

namespace HSEBank.BusinessLogic.Dto;

/// <summary>
/// Dto for getting category.
/// </summary>
public class GetCategoryDto
{
    public Guid Id { get; set; }
    public Type Type { get; set; }
    public string Name { get; set; }
}