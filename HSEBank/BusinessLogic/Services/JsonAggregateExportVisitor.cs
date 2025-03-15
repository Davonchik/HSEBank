using System.Text.Json;
using System.Text.Json.Nodes;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Services;

public class JsonAggregateExportVisitor : IDataExportVisitor
{
    private readonly List<IVisitable> _objects = [];

    public void SaveToFile(string filePath)
    {
        string json = JsonSerializer.Serialize(_objects.Cast<object>(), new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
        Console.WriteLine($"Экспортировано {_objects.Count} объектов в файл {filePath}");
    }

    public void Visit(IVisitable visitable)
    {
        _objects.Add(visitable);
    }
}