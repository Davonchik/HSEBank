using HSEBank.BusinessLogic.Services;
using HSEBank.BusinessLogic.Services.Abstractions;

namespace TestHSEBank;

public class YamlAggregateExportVisitorTests
{
    [Fact]
    public void SaveToFile_WritesEmptyList_WhenNoObjectsVisited()
    {
        // Arrange
        var visitor = new YamlAggregateExportVisitor();
        string tempFile = Path.GetTempFileName();

        try
        {
            // Act
            visitor.SaveToFile(tempFile);
            string content = File.ReadAllText(tempFile).Trim();

            // Assert
            // По умолчанию, YamlDotNet сериализует пустой список как "[]"
            Assert.Contains("[]", content);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void SaveToFile_WritesCorrectYaml_WhenObjectsVisited()
    {
        // Arrange
        var visitor = new YamlAggregateExportVisitor();
        var testObj1 = new TestVisitable { Name = "Alice" };
        var testObj2 = new TestVisitable { Name = "Bob" };
        visitor.Visit(testObj1);
        visitor.Visit(testObj2);
        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".yaml");

        try
        {
            // Act
            visitor.SaveToFile(tempFile);
            string content = File.ReadAllText(tempFile);

            // Assert: проверяем, что в YAML-выводе содержатся имена "Alice" и "Bob"
            Assert.Contains("Alice", content);
            Assert.Contains("Bob", content);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}