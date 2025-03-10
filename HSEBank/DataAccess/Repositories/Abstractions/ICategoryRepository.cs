using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;

namespace HSEBank.DataAccess.Repositories.Abstractions;

public interface ICategoryRepository : IRepository<Category, EditCategoryDto>;