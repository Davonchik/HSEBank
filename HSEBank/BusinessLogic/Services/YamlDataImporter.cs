using HSEBank.BusinessLogic.Services.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSEBank.BusinessLogic.Services;

/// <summary>
/// Importer for YAML / YML formats.
/// </summary>
/// <typeparam name="T"></typeparam>
public class YamlDataImporter<T> : IDataImporter<T>
{
    public List<T> Import(string filePath)
    {
        var yaml = File.ReadAllText(filePath);
        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();
        var data = deserializer.Deserialize<List<T>>(yaml);
        return data ?? [];
    }
}