namespace HSEBank.BusinessLogic.Services.Abstractions;

/// <summary>
/// Interface for data importing.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDataImporter<T>
{
    List<T> Import(string filePath);
}