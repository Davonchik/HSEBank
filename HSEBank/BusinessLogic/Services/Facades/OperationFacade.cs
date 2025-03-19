using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;

namespace HSEBank.BusinessLogic.Services.Facades;

/// <summary>
/// Implementation of Operation Facade (CRUD logic).
/// </summary>
public class OperationFacade : IOperationFacade
{
    private IFinancialFactory _financialFactory { get; }
    
    private IOperationRepository _operationRepository { get; }

    public OperationFacade(IFinancialFactory financialFactory, IOperationRepository operationRepository)
    {
        _financialFactory = financialFactory;
        _operationRepository = operationRepository;
    }
    
    public Operation Create(OperationDto operationDto)
    {
        var newOperation = _financialFactory.CreateOperation(operationDto);
        _operationRepository.Create(newOperation);
        return newOperation;
    }

    public Operation GetById(Guid id)
    {
        if (!OperationExists(id))
        {
            throw new ArgumentException($"Операции с таким ID'{id}' не существует!t");
        }
        return _operationRepository.GetById(id);
    }

    public bool EditOperation(EditOperationDto editOperationDto)
    {
        if (!OperationExists(editOperationDto.OperationId))
        {
            throw new ArgumentException($"Операции с таким ID'{editOperationDto.OperationId}' не существует!");
        }
        return _operationRepository.Update(editOperationDto);
    }

    public bool DeleteOperation(Guid id)
    {
        if (!OperationExists(id))
        {
            throw new ArgumentException($"Операции с ID'{id}' не существует!");
        }
        return _operationRepository.Delete(id);
    }

    public IEnumerable<Operation> GetAllOperations()
    {
        return _operationRepository.GetAll();
    }

    public IEnumerable<Operation> GetByCondition(Func<Operation, bool> condition)
    {
        var res = _operationRepository.GetByCondition(condition).ToList();
        if (res.Count == 0)
        {
            Console.WriteLine("Операции по запросу не найдены!");
        }
        return res;
    }

    public bool OperationExists(Guid id)
    {
        return _operationRepository.Exists(id);
    }
}