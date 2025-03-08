namespace HSEBank.Abstractions;

public interface IDataImporter<T>
{
    List<T> Import(string filePath);
    //TODO для остальных типов
}