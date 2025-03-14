using System.Globalization;
using System.Text.Json;
using CsvHelper;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Services.Facades;

public class CsvAggregateExportVisitor : IDataExportVisitor
{
    private readonly List<IVisitable> _objects = [];
    
    public void SaveToFile(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            
        // Записываем заголовок
        csv.WriteField("Type");
        csv.WriteField("Data");
        csv.NextRecord();
            
        // Для каждого объекта записываем его тип и данные (в виде JSON)
        foreach (var obj in _objects)
        {
            csv.WriteField(obj.GetType().Name);
            string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
            csv.WriteField(json);
            csv.NextRecord();
        }
        Console.WriteLine($"Экспортировано {_objects.Count} объектов в файл {filePath}");
    }
    
    public void Visit(IVisitable visitable)
    {
        _objects.Add(visitable);
    }
}