namespace HSEBank.Abstractions;

/// <summary>
/// An interface to support the Visitor pattern in domain models.
/// </summary>
public interface IVisitable
{
    void Accept(IDataExportVisitor visitor);
}