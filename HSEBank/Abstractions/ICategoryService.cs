using HSEBank.Dto;
using HSEBank.Models;

namespace HSEBank.Abstractions;

/// <summary>
/// Service for working with categories.
/// </summary>
public interface ICategoryService
{
    Category Create(CategoryDto dto);
    Category GetById(Guid id);
    bool Update(EditCategoryDto dto);
    bool Delete(Guid id);
    IEnumerable<Category> GetAll();
    IEnumerable<Category> GetByCondition(Func<Category, bool> predicate);
}