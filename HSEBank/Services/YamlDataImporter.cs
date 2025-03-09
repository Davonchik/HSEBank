using HSEBank.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSEBank.Services;

public class YamlDataImporter<T> : IDataImporter<T>
{
    public List<T> Import(string filePath)
    {
        var yaml = File.ReadAllText(filePath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var data = deserializer.Deserialize<List<T>>(yaml);
        return data ?? [];
    }
}