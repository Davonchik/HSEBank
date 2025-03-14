using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;

namespace HSEBank.BusinessLogic.Services.Facades;

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
            throw new ArgumentException($"Operation with id {id} does not exist");
        }
        return _operationRepository.GetById(id);
    }

    public bool EditOperation(EditOperationDto editOperationDto)
    {
        if (!OperationExists(editOperationDto.OperationId))
        {
            throw new ArgumentException($"Operation with id {editOperationDto.OperationId} does not exist");
        }
        return _operationRepository.Update(editOperationDto);
    }

    public bool DeleteOperation(Guid id)
    {
        if (!OperationExists(id))
        {
            throw new ArgumentException($"Operation with id {id} does not exist");
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