using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;

namespace HSEBank.BusinessLogic.Services.Facades;

/// <summary>
/// Implementation of Category Facade (CRUD logic).
/// </summary>
public class CategoryFacade : ICategoryFacade
{
    private IFinancialFactory _financialFactory { get; }
    
    private ICategoryRepository _categoryRepository { get; }

    public CategoryFacade(IFinancialFactory financialFactory, ICategoryRepository categoryRepository)
    {
        _financialFactory = financialFactory;
        _categoryRepository = categoryRepository;
    }

    public Category Create(CategoryDto categoryDto)
    {
        if (_categoryRepository.GetAll().Any(x => x.Name == categoryDto.Name && x.Type == categoryDto.Type))
        {
            throw new ArgumentException($"Категория с именем {categoryDto.Name} и типом {categoryDto.Type} уже существует");
        }
        var newCategory = _financialFactory.CreateCategory(categoryDto);
        _categoryRepository.Create(newCategory);
        return newCategory;
    }

    public Category GetById(Guid id)
    {
        if (!CategoryExists(id))
        {
            throw new ArgumentException($"Категории с таким ID'{id}' не существует!");
        }
        return _categoryRepository.GetById(id);
    }

    public bool EditCategory(EditCategoryDto dto)
    {
        if (!CategoryExists(dto.CategoryId))
        {
            throw new ArgumentException($"Категории с таким ID'{dto.CategoryId}' не существует!");
        }
        return _categoryRepository.Update(dto);
    }

    public bool DeleteCategory(Guid id)
    {
        if (!CategoryExists(id))
        {
            throw new ArgumentException($"Категории с таким ID'{id}' не существует!");
        }
        return _categoryRepository.Delete(id);
    }

    public IEnumerable<Category> GetAllCategories()
    {
        return _categoryRepository.GetAll();
    }

    public IEnumerable<Category> GetByCondition(Func<Category, bool> predicate)
    {
        if (!_categoryRepository.GetByCondition(predicate).Any())
        {
            throw new ArgumentException($"Нет подходящих по {predicate} операций!");
        }
        return _categoryRepository.GetByCondition(predicate);
    }

    public bool CategoryExists(Guid id)
    {
        return _categoryRepository.Exists(id);
    }
}