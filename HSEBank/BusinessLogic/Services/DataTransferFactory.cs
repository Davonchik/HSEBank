using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;

namespace HSEBank.BusinessLogic.Services;

/// <summary>
/// Factory for working with different types of files.
/// </summary>
public class DataTransferFactory
{
    public static IDataImporter<T> CreateImporter<T>(string filePath) where T : new()
    {
        string extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".json" => new JsonDataImporter<T>(),
            ".csv" => new CsvDataImporter<T>(),
            ".yaml" => new YamlDataImporter<T>(),
            ".yml" => new YamlDataImporter<T>(),
            _ => throw new NotSupportedException($"Формат файла {extension} не поддерживается.")
        };
    }
    
    public static IDataExportVisitor CreateExporter(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".json" => new JsonAggregateExportVisitor(),
            ".csv" => new CsvAggregateExportVisitor(),
            ".yaml" => new YamlAggregateExportVisitor(),
            ".yml" => new YamlAggregateExportVisitor(),
            _ => throw new NotSupportedException($"Формат файла {extension} не поддерживается.")
        };
    }
}