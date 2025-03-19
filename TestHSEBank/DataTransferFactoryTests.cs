using HSEBank.BusinessLogic.Services;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;

namespace TestHSEBank;

public class DataTransferFactoryTests
{
    // Фиктивный класс для тестирования импортеров
    private class TestData
    {
        public int Value { get; set; }
    }

    [Theory]
    [InlineData("data.json", typeof(JsonDataImporter<TestData>))]
    [InlineData("data.CSV",  typeof(CsvDataImporter<TestData>))]
    [InlineData("data.yaml", typeof(YamlDataImporter<TestData>))]
    [InlineData("data.yml",  typeof(YamlDataImporter<TestData>))]
    public void CreateImporter_ShouldReturn_CorrectDataImporter(string filePath, Type expectedType)
    {
        // Act
        IDataImporter<TestData> importer = DataTransferFactory.CreateImporter<TestData>(filePath);

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
        var exception = Assert.Throws<NotSupportedException>(() => DataTransferFactory.CreateImporter<TestData>(filePath));
        string extension = Path.GetExtension(filePath).ToLower();
        Assert.Equal($"Формат файла {extension} не поддерживается.", exception.Message);
    }

    [Theory]
    [InlineData("export.json", typeof(JsonAggregateExportVisitor))]
    [InlineData("export.csv",  typeof(CsvAggregateExportVisitor))]
    [InlineData("export.yaml", typeof(YamlAggregateExportVisitor))]
    [InlineData("export.yml",  typeof(YamlAggregateExportVisitor))]
    public void CreateExporter_ShouldReturn_CorrectVisitor(string filePath, Type expectedType)
    {
        // Act
        IDataExportVisitor visitor = DataTransferFactory.CreateExporter(filePath);

        // Assert
        Assert.NotNull(visitor);
        Assert.IsType(expectedType, visitor);
    }

    [Theory]
    [InlineData("export.xml")]
    [InlineData("export.txt")]
    [InlineData("export.doc")]
    public void CreateExporter_ShouldThrow_NotSupportedException_ForUnsupportedExtension(string filePath)
    {
        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() => DataTransferFactory.CreateExporter(filePath));
        string extension = Path.GetExtension(filePath).ToLower();
        Assert.Equal($"Формат файла {extension} не поддерживается.", exception.Message);
    }
}