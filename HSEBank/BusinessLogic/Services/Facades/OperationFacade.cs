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
        return _operationRepository.GetById(id);
    }

    public bool EditOperation(EditOperationDto editOperationDto)
    {
        return _operationRepository.Update(editOperationDto);
    }

    public bool DeleteOperation(Guid id)
    {
        return _operationRepository.Delete(id);
    }

    public IEnumerable<Operation> GetAllOperations()
    {
        return _operationRepository.GetAll();
    }

    public IEnumerable<Operation> GetByCondition(Func<Operation, bool> condition)
    {
        return _operationRepository.GetByCondition(condition);
    }

    public bool OperationExists(Guid id)
    {
        return _operationRepository.OperationIsExist(id);
    }
}