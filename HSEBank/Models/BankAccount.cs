using HSEBank.Abstractions;

namespace HSEBank.Models;

/// <summary>
/// Bank account model with support for the Visitor pattern.
/// </summary>
public class BankAccount : IVisitable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }

    public void Accept(IDataExportVisitor visitor)
    {
        visitor.Visit(this);
    }
}