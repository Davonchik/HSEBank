using HSEBank.Abstractions;
using HSEBank.Dto;
using HSEBank.Models;

namespace HSEBank.Services;

public class OperationService : IOperationService
{
    private readonly Dictionary<Guid, Operation> _operations = new();
    
    public Operation Create(OperationDto dto)
    {
        var operation = new Operation
        {
            Id = Guid.NewGuid(),
            Amount = dto.Amount,
            Description = dto.Description,
            Type = dto.Type,
            BankAccountId = dto.BankAccountId,
            CategoryId = dto.CategoryId,
            Date = DateTime.Now
        };
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

    public bool OperationIsExist(Guid operationId)
    {
        return _operations.ContainsKey(operationId);
    }
}