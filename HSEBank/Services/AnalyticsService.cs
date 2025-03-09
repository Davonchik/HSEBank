using HSEBank.Abstractions;
using HSEBank.Dto;
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

    public decimal GetBalanceDifference(FinancialDataDto data, DateTime startDate, DateTime endDate)
    {
        decimal totalIncome = data.Operations
            .Where(op => op.Date >= startDate && op.Date <= endDate && op.Type == OperationType.Income)
            .Sum(op => op.Amount);
        decimal totalExpense = data.Operations
            .Where(op => op.Date >= startDate && op.Date <= endDate && op.Type == OperationType.Expense)
            .Sum(op => op.Amount);
        return totalIncome - totalExpense;
    }

    public Dictionary<Guid, List<Operation>> GroupOperationsByCategory(FinancialDataDto data)
    {
        return data.Operations
            .GroupBy(op => op.CategoryId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}