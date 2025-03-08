using HSEBank.Abstractions;

namespace HSEBank.Models;

public enum CategoryType
{
    Income,
    Expense,
}

/// <summary>
/// Category model (income/expense) with support for the Visitor pattern.
/// </summary>
public class Category : IVisitable
{
    public Guid Id { get; set; }
    public CategoryType Type { get; set; }
    public string Name { get; set; }
    
    public void Accept(IDataExportVisitor visitor)
    {
        visitor.Visit(this);
    }
}