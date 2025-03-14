using HSEBank.BusinessLogic.Services.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSEBank.BusinessLogic.Services;

public class YamlAggregateExportVisitor
{
    private readonly List<IVisitable> _objects = [];
    
    public void SaveToFile(string filePath)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
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