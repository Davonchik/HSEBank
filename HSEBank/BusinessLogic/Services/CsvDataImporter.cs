using System.Globalization;
using System.Text.Json;
using CsvHelper;
using HSEBank.BusinessLogic.Services.Abstractions;

namespace HSEBank.BusinessLogic.Services;

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
        var data = csv.Read();
        csv.ReadHeader();
        List<T> results = [];
        while (csv.Read())
        {
            var jsonData = csv.GetField("Data");
            if (jsonData == null)
            {
                throw new InvalidDataException("В файле csv отсутствует поле Data!");
            }
            var obj = JsonSerializer.Deserialize<T>(jsonData);

            if (obj != null)
                results.Add(obj);
        }

        return results;
    }
}