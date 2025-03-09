using CsvHelper;
using System.Globalization;
using HSEBank.Abstractions;

namespace HSEBank.Services;

/// <summary>
/// Importer for CSV format using CsvHelper.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CsvDataImporter<T> : IDataImporter<T>
{
    public List<T> Import(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<T>().ToList();
        return records;
    }
}