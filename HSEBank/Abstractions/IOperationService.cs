using HSEBank.Models;
using HSEBank.Dto;

namespace HSEBank.Abstractions;

/// <summary>
/// Service for working with operations.
/// </summary>
public interface IOperationService
{
    Operation Create(OperationDto dto);
    Operation GetById(Guid id);
    bool Update(EditOperationDto dto);
    bool Delete(Guid id);
    IEnumerable<Operation> GetAll();
    IEnumerable<Operation> GetByCondition(Func<Operation, bool> predicate);
    bool OperationIsExist(Guid operationId);
}