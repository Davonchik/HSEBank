using HSEBank.Abstractions;
using HSEBank.Models;
using HSEBank.Shared;

namespace HSEBank.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IFinancialAssistant _assistant;

    public AnalyticsService(IFinancialAssistant assistant)
    {
        _assistant = assistant;
    }

    public decimal GetBalanceDifference(DateTime start, DateTime end)
    {
        decimal totalIncome = 0;
        decimal totalExpense = 0;
        foreach (var op in _assistant.GetAllOperations())
        {
            if (op.Date >= start && op.Date <= end)
            {
                if (op.Type == OperationType.Income)
                    totalIncome += op.Amount;
                else if (op.Type == OperationType.Expense)
                    totalExpense += op.Amount;
            }
        }
        return totalIncome - totalExpense;
    }

    public Dictionary<Guid, List<Operation>> GroupOperationsByCategory()
    {
        var result = new Dictionary<Guid, List<Operation>>();
        foreach (var op in _assistant.GetAllOperations())
        {
            if (!result.ContainsKey(op.CategoryId))
                result[op.CategoryId] = new List<Operation>();
            result[op.CategoryId].Add(op);
        }
        return result;
    }
}