using HSEBank.Abstractions;

namespace HSEBank.Services;

/// <summary>
/// Importer for CSV format.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CsvDataImporter<T> : IDataImporter<T> where T : new()
{
    public List<T> Import(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        return lines.Skip(1).Select(line => new T()).ToList();
    }
}