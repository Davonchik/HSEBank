using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Services.Facades;

/// <summary>
/// Operation Facade interface.
/// </summary>
public interface IOperationFacade
{
    public Operation Create(OperationDto operationDto);

    public Operation GetById(Guid id);

    public bool EditOperation(EditOperationDto editOperationDto);

    public bool DeleteOperation(Guid id);

    public IEnumerable<Operation> GetAllOperations();

    public IEnumerable<Operation> GetByCondition(Func<Operation, bool> condition);

    public bool OperationExists(Guid id);
}