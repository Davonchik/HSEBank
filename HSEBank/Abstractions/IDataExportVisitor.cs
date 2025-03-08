using HSEBank.Models;

namespace HSEBank.Abstractions;

/// <summary>
/// Visitor interface for exporting data from domain objects.
/// </summary>
public interface IDataExportVisitor
{
    void Visit(BankAccount account);
    void Visit(Category category);
    void Visit(Operation operation);
}