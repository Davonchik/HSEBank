namespace HSEBank.Abstractions;

/// <summary>
/// Service for working with operations (checking existence and advanced CRUD operations).
/// </summary>
public interface IOperationService
{
    //TODO CRUD
    bool OperationIsExist(Guid operationId);
}