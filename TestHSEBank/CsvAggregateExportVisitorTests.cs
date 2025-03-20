using System.Globalization;
using System.Text.Json;
using CsvHelper;
using HSEBank.BusinessLogic.Services.Facades;

namespace TestHSEBank;

public class CsvAggregateExportVisitorTests
{
    [Fact]
    public void SaveToFile_WritesHeaderOnly_WhenNoObjectsVisited()
    {
        // Arrange
        var visitor = new CsvAggregateExportVisitor();
        string tempFile = Path.GetTempFileName();

        try
        {
            // Act
            visitor.SaveToFile(tempFile);
            string content = File.ReadAllText(tempFile).Trim();
            
            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Assert
            Assert.Single(lines);
            Assert.Contains("Type", lines[0]);
            Assert.Contains("Data", lines[0]);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void SaveToFile_WritesVisitedObjects_Correctly()
    {
        // Arrange
        var visitor = new CsvAggregateExportVisitor();
        var obj1 = new TestVisitable { Name = "Value1" };
        var obj2 = new TestVisitable { Name = "Value2" };

        visitor.Visit(obj1);
        visitor.Visit(obj2);

        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csv");

        try
        {
            // Act
            visitor.SaveToFile(tempFile);

            using var reader = new StreamReader(tempFile);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            csvReader.Read();
            csvReader.ReadHeader();

            csvReader.Read();
            string typeField1 = csvReader.GetField("Type");
            string dataField1 = csvReader.GetField("Data");

            csvReader.Read();
            string typeField2 = csvReader.GetField("Type");
            string dataField2 = csvReader.GetField("Data");

            Assert.Equal("TestVisitable", typeField1);
            Assert.Equal("TestVisitable", typeField2);

            var deserialized1 = JsonSerializer.Deserialize<TestVisitable>(dataField1);
            var deserialized2 = JsonSerializer.Deserialize<TestVisitable>(dataField2);

            Assert.Equal("Value1", deserialized1.Name);
            Assert.Equal("Value2", deserialized2.Name);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}