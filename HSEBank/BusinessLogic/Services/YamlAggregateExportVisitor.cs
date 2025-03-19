using HSEBank.BusinessLogic.Services.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSEBank.BusinessLogic.Services;

/// <summary>
/// YAML / YML files export logic.
/// </summary>
public class YamlAggregateExportVisitor : IDataExportVisitor
{
    private readonly List<IVisitable> _objects = [];
    
    public void SaveToFile(string filePath)
    {
        var serializer = new SerializerBuilder()
            .Build();
        string yaml = serializer.Serialize(_objects);
        File.WriteAllText(filePath, yaml);
        Console.WriteLine($"Экспортировано {_objects.Count} объектов в файл {filePath}");
    }
    
    public void Visit(IVisitable visitable)
    {
        _objects.Add(visitable);
    }
}