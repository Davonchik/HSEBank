using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services;
using HSEBank.DataAccess.Models;

using Type = HSEBank.DataAccess.Common.Enums.Type;

namespace TestHSEBank;

public class AnalyticsServiceTests
{
    private readonly AnalyticsService _analyticsService;
    private readonly Fixture _fixture;

    public AnalyticsServiceTests()
    {
        _analyticsService = new AnalyticsService();
        _fixture = new Fixture();
    }

    [Fact]
    public void GetBalanceDifference_Returns_Correct_Difference()
    {
        // Arrange
        DateTime start = new DateTime(2023, 1, 1);
        DateTime end = new DateTime(2023, 1, 31);
        
        // Создаем операции, где только операции в январе учитываются
        var operations = new List<Operation>
        {
            new Operation { Amount = 100, Date = new DateTime(2023, 1, 5), Type = Type.Income },
            new Operation { Amount = 50,  Date = new DateTime(2023, 1, 10), Type = Type.Expense },
            new Operation { Amount = 200, Date = new DateTime(2023, 1, 15), Type = Type.Income },
            new Operation { Amount = 75,  Date = new DateTime(2023, 1, 20), Type = Type.Expense },
            // Операции вне диапазона
            new Operation { Amount = 300, Date = new DateTime(2023, 2, 1), Type = Type.Income },
            new Operation { Amount = 100, Date = new DateTime(2022, 12, 31), Type = Type.Expense }
        };
        var data = new FinancialDataDto { Operations = operations };

        // В диапазоне: доход = 100+200 = 300, расход = 50+75 = 125, разница = 175.
        decimal expectedDifference = 175;

        // Act
        var difference = _analyticsService.GetBalanceDifference(data, start, end);

        // Assert
        Assert.Equal(expectedDifference, difference);
    }

    [Fact]
    public void GetBalanceDifference_Returns_Zero_When_No_Operations_In_Range()
    {
        // Arrange
        DateTime start = new DateTime(2023, 1, 1);
        DateTime end = new DateTime(2023, 1, 31);
        var operations = new List<Operation>
        {
            new Operation { Amount = 100, Date = new DateTime(2022, 12, 31), Type = Type.Income },
            new Operation { Amount = 50, Date = new DateTime(2023, 2, 1), Type = Type.Expense }
        };
        var data = new FinancialDataDto { Operations = operations };

        // Act
        var difference = _analyticsService.GetBalanceDifference(data, start, end);

        // Assert
        Assert.Equal(0, difference);
    }

    [Fact]
    public void GroupOperationsByCategory_Returns_Correct_Grouping()
    {
        // Arrange
        Guid cat1 = Guid.NewGuid();
        Guid cat2 = Guid.NewGuid();
        var operations = new List<Operation>
        {
            new Operation { CategoryId = cat1, Amount = 100, Date = DateTime.Now, Type = Type.Income },
            new Operation { CategoryId = cat1, Amount = 50,  Date = DateTime.Now, Type = Type.Expense },
            new Operation { CategoryId = cat2, Amount = 200, Date = DateTime.Now, Type = Type.Income }
        };
        var data = new FinancialDataDto { Operations = operations };

        // Act
        var grouping = _analyticsService.GroupOperationsByCategory(data);

        // Assert
        Assert.Equal(2, grouping.Count);
        Assert.True(grouping.ContainsKey(cat1));
        Assert.True(grouping.ContainsKey(cat2));
        Assert.Equal(2, grouping[cat1].Count); // две операции для cat1
        Assert.Single(grouping[cat2]);         // одна операция для cat2
    }

    [Fact]
    public void GroupOperationsByCategory_Returns_Empty_Dictionary_When_No_Operations()
    {
        // Arrange
        var data = new FinancialDataDto { Operations = new List<Operation>() };

        // Act
        var grouping = _analyticsService.GroupOperationsByCategory(data);

        // Assert
        Assert.Empty(grouping);
    }
}