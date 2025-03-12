using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;

namespace HSEBank.BusinessLogic.Services.Facades;

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
            throw new ArgumentException($"Category with name {categoryDto.Name} and type {categoryDto.Type} already exists");
        }
        var newCategory = _financialFactory.CreateCategory(categoryDto);
        _categoryRepository.Create(newCategory);
        return newCategory;
    }

    public Category GetById(Guid id)
    {
        if (!CategoryExists(id))
        {
            throw new ArgumentException($"Category with id {id} does not exist");
        }
        return _categoryRepository.GetById(id);
    }

    public bool EditCategory(EditCategoryDto dto)
    {
        if (!CategoryExists(dto.CategoryId))
        {
            throw new ArgumentException($"Category with id {dto.CategoryId} does not exist");
        }
        return _categoryRepository.Update(dto);
    }

    public bool DeleteCategory(Guid id)
    {
        if (!CategoryExists(id))
        {
            throw new ArgumentException($"Category with id {id} does not exist");
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
            throw new ArgumentException($"No operations found for condition {predicate}");
        }
        return _categoryRepository.GetByCondition(predicate);
    }

    public bool CategoryExists(Guid id)
    {
        return _categoryRepository.Exists(id);
    }
}