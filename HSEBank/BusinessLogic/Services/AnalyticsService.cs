using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;
using Type = HSEBank.DataAccess.Common.Enums.Type;

namespace HSEBank.BusinessLogic.Services;

/// <summary>
/// Service for analytics work.
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    public decimal GetBalanceDifference(FinancialDataDto data, DateTime startDate, DateTime endDate)
    {
        decimal totalIncome = data.Operations
            .Where(op => op.Date >= startDate && op.Date <= endDate && op.Type == Type.Income)
            .Sum(op => op.Amount);
        decimal totalExpense = data.Operations
            .Where(op => op.Date >= startDate && op.Date <= endDate && op.Type == Type.Expense)
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