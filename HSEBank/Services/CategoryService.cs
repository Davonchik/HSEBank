using HSEBank.Abstractions;
using HSEBank.Dto;
using HSEBank.Models;

namespace HSEBank.Services;

/// <summary>
/// Category service.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly Dictionary<Guid, Category> _categories = new();
    
    public Category Create(CategoryDto dto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Type = dto.Type,
            Name = dto.Name,
        };
        _categories[category.Id] = category;
        return category;
    }

    public Category GetById(Guid id)
    {
        return _categories.TryGetValue(id, out var categ) ? categ : throw new KeyNotFoundException("Категория не найдена");
    }

    public bool Update(EditCategoryDto dto)
    {
        if (_categories.ContainsKey(dto.CategoryId))
        {
            _categories[dto.CategoryId].Name = dto.Name;
            return true;
        }

        return false;
    }

    public bool Delete(Guid id)
    {
        return _categories.Remove(id);
    }

    public IEnumerable<Category> GetAll()
    {
        return _categories.Values;
    }

    public IEnumerable<Category> GetByCondition(Func<Category, bool> predicate)
    {
        return _categories.Values.Where(predicate);
    }
}