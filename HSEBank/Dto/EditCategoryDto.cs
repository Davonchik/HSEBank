namespace HSEBank.Dto;

/// <summary>
/// Dto for editing category.
/// </summary>
public class EditCategoryDto
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
}