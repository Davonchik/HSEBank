using System.Text.Json;
using HSEBank.BusinessLogic.Services;
using HSEBank.BusinessLogic.Services.Abstractions;

namespace TestHSEBank;

public class TestVisitable : IVisitable
{
    public string Name { get; set; }
    public void Accept(IDataExportVisitor visitor)
    {
        
    }
}

public class JsonAggregateExportVisitorTests
{
    [Fact]
    public void SaveToFile_Writes_Empty_Array_When_No_Objects_Visited()
    {
        // Arrange
        var visitor = new JsonAggregateExportVisitor();
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            visitor.SaveToFile(tempFile);
            var content = File.ReadAllText(tempFile);

            // Assert: Ожидается, что при отсутствии объектов будет записан пустой массив "[]"
            Assert.Equal("[]", content.Trim());
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void SaveToFile_Writes_Correct_Json_When_Objects_Visited()
    {
        // Arrange
        var visitor = new JsonAggregateExportVisitor();
        var testObject1 = new TestVisitable { Name = "Alice" };
        var testObject2 = new TestVisitable { Name = "Bob" };
        visitor.Visit(testObject1);
        visitor.Visit(testObject2);
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            visitor.SaveToFile(tempFile);
            var content = File.ReadAllText(tempFile);

            // Для проверки десериализуем JSON в список JsonElement, чтобы избежать проблем с привязкой типов.
            var deserialized = JsonSerializer.Deserialize<List<JsonElement>>(content);
            
            // Assert: Ожидается массив с двумя объектами, содержащими свойства "Name"
            Assert.Equal(2, deserialized.Count);
            bool containsAlice = deserialized.Any(e =>
                e.TryGetProperty("Name", out JsonElement prop) && prop.GetString() == "Alice");
            bool containsBob = deserialized.Any(e =>
                e.TryGetProperty("Name", out JsonElement prop) && prop.GetString() == "Bob");
            Assert.True(containsAlice && containsBob);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}