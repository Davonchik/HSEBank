using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;

namespace HSEBank.DataAccess.Repositories;

/// <summary>
/// Operation service.
/// </summary>
public class OperationRepository : IOperationRepository
{
    private readonly Dictionary<Guid, Operation> _operations = new();
    
    public Operation Create(Operation operation)
    {
        _operations[operation.Id] = operation;
        return operation;
    }

    public Operation GetById(Guid id)
    {
        return _operations.TryGetValue(id, out var op) ? op : throw new KeyNotFoundException("Операция не найдена");
    }

    public bool Update(EditOperationDto dto)
    {
        if (_operations.ContainsKey(dto.OperationId))
        {
            _operations[dto.OperationId].CategoryId = dto.CategoryId;
            return true;
        }
        
        return false;
    }

    public bool Delete(Guid id)
    {
        return _operations.Remove(id);
    }

    public IEnumerable<Operation> GetAll()
    {
        return _operations.Values;
    }

    public IEnumerable<Operation> GetByCondition(Func<Operation, bool> predicate)
    {
        return _operations.Values.Where(predicate);
    }

    public bool Exists(Guid operationId)
    {
        return _operations.ContainsKey(operationId);
    }
}