using HSEBank.BusinessLogic.Services;
using HSEBank.BusinessLogic.Services.Abstractions;

namespace TestHSEBank;

public class DataImporterFactoryTests
{
    private class TestData
    {
        public int Value { get; set; }
    }

    [Theory]
    [InlineData("data.json", typeof(JsonDataImporter<TestData>))]
    [InlineData("data.CSV", typeof(CsvDataImporter<TestData>))]
    [InlineData("data.yaml", typeof(YamlDataImporter<TestData>))]
    [InlineData("data.yml", typeof(YamlDataImporter<TestData>))]
    public void CreateImporter_ShouldReturn_CorrectDataImporter(string filePath, Type expectedType)
    {
        // Act
        IDataImporter<TestData> importer = DataImporterFactory.CreateImporter<TestData>(filePath);

        // Assert
        Assert.NotNull(importer);
        Assert.IsType(expectedType, importer);
    }

    [Theory]
    [InlineData("data.xml")]
    [InlineData("data.txt")]
    [InlineData("data.doc")]
    public void CreateImporter_ShouldThrow_NotSupportedException_ForUnsupportedExtension(string filePath)
    {
        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() => DataImporterFactory.CreateImporter<TestData>(filePath));
        string extension = Path.GetExtension(filePath).ToLower();
        Assert.Equal($"Формат файла {extension} не поддерживается.", exception.Message);
    }
}