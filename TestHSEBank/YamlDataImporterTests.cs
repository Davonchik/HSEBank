using HSEBank.BusinessLogic.Services;

namespace TestHSEBank;

public class YamlDataImporterTests
{
    private class TestData
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    [Fact]
    public void Import_ShouldReturnList_WhenYamlIsValid()
    {
        // Arrange: создаём YAML-строку с данными
        var yamlContent = @"- name: Alice
  value: 123
- name: Bob
  value: 456";

        // Создаём временный файл
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, yamlContent);

        var importer = new YamlDataImporter<TestData>();

        // Act: выполняем импорт данных из файла
        var result = importer.Import(tempFile);

        // Assert: проверяем, что данные импортированы корректно
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal(123, result[0].Value);
        Assert.Equal("Bob", result[1].Name);
        Assert.Equal(456, result[1].Value);

        // Clean up: удаляем временный файл
        File.Delete(tempFile);
    }

    [Fact]
    public void Import_ShouldReturnEmptyList_WhenYamlIsEmpty()
    {
        // Arrange: создаём пустой файл
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, string.Empty);

        var importer = new YamlDataImporter<TestData>();

        // Act: импортируем данные из пустого файла
        var result = importer.Import(tempFile);

        // Assert: результат не null, но пустой список
        Assert.NotNull(result);
        Assert.Empty(result);

        // Clean up
        File.Delete(tempFile);
    }

    [Fact]
    public void Import_ShouldThrowException_WhenFileDoesNotExist()
    {
        // Arrange: формируем путь к несуществующему файлу
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".yaml");
        var importer = new YamlDataImporter<TestData>();

        // Act & Assert: ожидаем исключение FileNotFoundException
        Assert.Throws<FileNotFoundException>(() => importer.Import(nonExistentFile));
    }
}