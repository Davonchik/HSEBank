using System.Text.Json;
using System.Text.Json.Serialization;
using HSEBank.Abstractions;

namespace HSEBank.Services;

/// <summary>
/// Importer for JSON format.
/// </summary>
/// <typeparam name="T"></typeparam>
public class JsonDataImporter<T> : IDataImporter<T>
{
    public List<T> Import(string filePath)
    {
        string data = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<T>>(data)?? new List<T>();
    }
}