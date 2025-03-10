using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;

namespace HSEBank.DataAccess.Repositories;

/// <summary>
/// Category service.
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly Dictionary<Guid, Category> _categories = new();
    
    public Category Create(Category category)
    {
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