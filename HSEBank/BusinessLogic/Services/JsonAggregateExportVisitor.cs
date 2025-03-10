using System.Text.Json;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Services;

public class JsonAggregateExportVisitor : IDataExportVisitor
{
    private readonly List<object> _objects = new List<object>();

    public void Visit(BankAccount account)
    {
        _objects.Add(account);
    }

    public void Visit(Category category)
    {
        _objects.Add(category);
    }

    public void Visit(Operation operation)
    {
        _objects.Add(operation);
    }

    public void SaveToFile(string filePath)
    {
        string json = JsonSerializer.Serialize(_objects, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
        Console.WriteLine($"Экспортировано {_objects.Count} объектов в файл {filePath}");
    }
}