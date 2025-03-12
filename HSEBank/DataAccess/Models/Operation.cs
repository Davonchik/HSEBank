using HSEBank.BusinessLogic.Services.Abstractions;
using Type = HSEBank.DataAccess.Common.Enums.Type;

namespace HSEBank.DataAccess.Models;

/// <summary>
/// Operation model with support for the Visitor pattern.
/// </summary>
public class Operation : IVisitable
{
    public Guid Id { get; set; }
    public Type Type { get; set; }
    public Guid BankAccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }

    public void Accept(IDataExportVisitor visitor)
    {
        visitor.Visit(this);
    }
}