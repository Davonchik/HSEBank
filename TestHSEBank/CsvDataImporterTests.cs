using HSEBank.BusinessLogic.Services;

namespace TestHSEBank;

public class CsvDataImporterTests
{
    public class TestData
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    [Fact]
    public void Import_ShouldReturnList_WhenCsvIsValid()
    {
        // Arrange
        // Ожидаемый CSV:
        // Data
        // "{""Name"":""Alice"",""Value"":123}"
        // "{""Name"":""Bob"",""Value"":456}"
        var csvContent = @"Data
""{""""Name"""":""""Alice"""",""""Value"""":123}""
""{""""Name"""":""""Bob"""",""""Value"""":456}""";
    
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, csvContent);

        var importer = new CsvDataImporter<TestData>();

        // Act
        var result = importer.Import(tempFile);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal(123, result[0].Value);
        Assert.Equal("Bob", result[1].Name);
        Assert.Equal(456, result[1].Value);

        // Clean up
        File.Delete(tempFile);
    }

    [Fact]
    public void Import_ShouldThrowException_WhenDataFieldMissing()
    {
        // Arrange
        var csvContent = "Foo\r\n" +
                         "\"{\\\"Name\\\":\\\"Alice\\\",\\\"Value\\\":123}\"";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, csvContent);

        var importer = new CsvDataImporter<TestData>();

        // Act & Assert
        var ex = Assert.Throws<CsvHelper.MissingFieldException>(() => importer.Import(tempFile));
        Assert.Contains("Field with name 'Data' does not exist", ex.Message);

        // Clean up
        File.Delete(tempFile);
    }
}