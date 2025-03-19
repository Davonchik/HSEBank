using System.Diagnostics;
using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Services;

/// <summary>
/// Logger for analytics.
/// </summary>
/// <param name="service"></param>
public class AnalyticsServiceLoggerProxy(IAnalyticsService service) : IAnalyticsService
{
    private IAnalyticsService _service = service;

    public decimal GetBalanceDifference(FinancialDataDto data, DateTime start, DateTime end)
    {
        var watch = Stopwatch.StartNew();
        Console.WriteLine($"Запущен метод {nameof(GetBalanceDifference)}");
        var result = _service.GetBalanceDifference(data, start, end);
        watch.Stop();
        Console.WriteLine($"Метод {nameof(GetBalanceDifference)} завершен за {watch.ElapsedMilliseconds} мс");
        return result;
    }

    public Dictionary<Guid, List<Operation>> GroupOperationsByCategory(FinancialDataDto data)
    {
        var watch = Stopwatch.StartNew();
        Console.WriteLine($"Запущен метод {nameof(GroupOperationsByCategory)}");
        var result = _service.GroupOperationsByCategory(data);
        watch.Stop();
        Console.WriteLine($"Метод {nameof(GroupOperationsByCategory)} завершен за {watch.ElapsedMilliseconds} мс");
        return result;
    }
}