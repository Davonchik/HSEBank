using HSEBank.Dto;
using HSEBank.Models;

namespace HSEBank.Abstractions;

/// <summary>
/// Interface of the analytics service for calculating the difference between income and expenses, grouping by categories, etc.
/// </summary>
public interface IAnalyticsService
{
    decimal GetBalanceDifference(FinancialDataDto data, DateTime start, DateTime end);
    Dictionary<Guid, List<Operation>> GroupOperationsByCategory(FinancialDataDto data);
}