using HSEBank.Models;

namespace HSEBank.Dto;

/// <summary>
/// Dto for getting category.
/// </summary>
public class GetCategoryDto
{
    public Guid Id { get; set; }
    public CategoryType Type { get; set; }
    public string Name { get; set; }
}