using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Services.Facades;
//TODO: убрать модификаторы доступа

public interface ICategoryFacade
{
    public Category Create(CategoryDto categoryDto);

    public Category GetById(Guid id);

    public bool EditCategory(EditCategoryDto dto);

    public bool DeleteCategory(Guid id);

    public IEnumerable<Category> GetAllCategories();
    
    public bool CategoryExists(Guid id);

    public IEnumerable<Category> GetByCondition(Func<Category, bool> predicate);
}