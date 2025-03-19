using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;

namespace HSEBank.DataAccess.Repositories.Abstractions;

/// <summary>
/// Service for working with categories.
/// </summary>
public interface ICategoryRepository : IRepository<Category, EditCategoryDto>;