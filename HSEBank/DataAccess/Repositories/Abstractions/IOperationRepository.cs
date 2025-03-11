using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;

namespace HSEBank.DataAccess.Repositories.Abstractions;

/// <summary>
/// Service for working with operations.
/// </summary>
public interface IOperationRepository : IRepository<Operation, EditOperationDto>;