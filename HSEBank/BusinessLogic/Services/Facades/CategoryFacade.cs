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
        var newCategory = _financialFactory.CreateCategory(categoryDto);
        _categoryRepository.Create(newCategory);
        return newCategory;
    }

    public Category GetById(Guid id)
    {
        return _categoryRepository.GetById(id);
    }

    public bool EditCategory(EditCategoryDto dto)
    {
        return _categoryRepository.Update(dto);
    }

    public bool DeleteCategory(Guid id)
    {
        return _categoryRepository.Delete(id);
    }

    public IEnumerable<Category> GetAllCategories()
    {
        return _categoryRepository.GetAll();
    }

    public IEnumerable<Category> GetByCondition(Func<Category, bool> predicate)
    {
        return _categoryRepository.GetByCondition(predicate);
    }
    
    
}