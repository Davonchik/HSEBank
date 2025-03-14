using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Services.Abstractions;

/// <summary>
/// Visitor interface for exporting data from domain objects.
/// </summary>
public interface IDataExportVisitor
{
    public void SaveToFile(string filePath);
    
    void Visit(IVisitable visitable);
}