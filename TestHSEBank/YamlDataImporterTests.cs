using HSEBank.BusinessLogic.Services;

namespace TestHSEBank;

public class YamlDataImporterTests
{
    // Сделаем тип TestData публичным, чтобы десериализатор мог его корректно обрабатывать.
    public class TestData
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    [Fact]
    public void Import_ShouldReturnList_WhenYamlIsValid()
    {
        // Arrange: создаём YAML-строку с данными, используя явные символы новой строки.
        var yamlContent = "- Name: \"Alice\"\n  Value: 123\n- Name: \"Bob\"\n  Value: 456";
        
        // Создаём временный файл и записываем в него YAML.
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, yamlContent);

        var importer = new YamlDataImporter<TestData>();

        // Act: выполняем импорт данных из файла.
        var result = importer.Import(tempFile);

        // Assert: проверяем, что данные импортированы корректно.
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal(123, result[0].Value);
        Assert.Equal("Bob", result[1].Name);
        Assert.Equal(456, result[1].Value);

        // Clean up: удаляем временный файл.
        File.Delete(tempFile);
    }

    [Fact]
    public void Import_ShouldReturnEmptyList_WhenYamlIsEmpty()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, string.Empty);

        var importer = new YamlDataImporter<TestData>();

        // Act
        var result = importer.Import(tempFile);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        // Clean up.
        File.Delete(tempFile);
    }

    [Fact]
    public void Import_ShouldThrowException_WhenFileDoesNotExist()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".yaml");
        var importer = new YamlDataImporter<TestData>();

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => importer.Import(nonExistentFile));
    }
}