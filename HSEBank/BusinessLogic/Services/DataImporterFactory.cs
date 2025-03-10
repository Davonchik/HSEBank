using HSEBank.BusinessLogic.Services.Abstractions;

namespace HSEBank.BusinessLogic.Services;

public class DataImporterFactory
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
}