using HSEBank.Abstractions;

namespace HSEBank.Services;

public class DataImporterFactory
{
    public static IDataImporter<T> CreateImporter<T>(string filePath) where T : new()
    {
        string extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".json" => new JsonDataImporter<T>(),
            ".csv" => new CsvDataImporter<T>(),
            _ => throw new NotSupportedException($"Формат файла {extension} не поддерживается.")
        };
    }
}