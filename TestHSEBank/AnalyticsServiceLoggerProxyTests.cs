using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;
using Moq;

namespace TestHSEBank;

public class AnalyticsServiceLoggerProxyTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IAnalyticsService> _serviceMock;
    private readonly AnalyticsServiceLoggerProxy _proxy;

    public AnalyticsServiceLoggerProxyTests()
    {
        _fixture = new Fixture();
        _serviceMock = new Mock<IAnalyticsService>();
        _proxy = new AnalyticsServiceLoggerProxy(_serviceMock.Object);
    }

    [Fact]
    public void GetBalanceDifference_Should_Log_And_Delegate_To_Service()
    {
        // Arrange
        var data = _fixture.Create<FinancialDataDto>();
        DateTime start = DateTime.Now.AddDays(-10);
        DateTime end = DateTime.Now;
        decimal expectedResult = 123.45m;

        _serviceMock
            .Setup(s => s.GetBalanceDifference(data, start, end))
            .Returns(expectedResult);

        // Перехватываем вывод в консоль
        using var writer = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(writer);

        // Act
        var result = _proxy.GetBalanceDifference(data, start, end);

        // Восстанавливаем консоль
        Console.SetOut(originalOut);

        // Assert
        Assert.Equal(expectedResult, result);

        // Проверяем, что метод был делегирован
        _serviceMock.Verify(s => s.GetBalanceDifference(data, start, end), Times.Once);

        // Проверяем, что логирование действительно было
        var output = writer.GetStringBuilder().ToString();
        Assert.Contains("Запущен метод GetBalanceDifference", output);
        Assert.Contains("Метод GetBalanceDifference завершен за", output);
    }

    [Fact]
    public void GroupOperationsByCategory_Should_Log_And_Delegate_To_Service()
    {
        // Arrange
        var data = _fixture.Create<FinancialDataDto>();
        var expectedDict = new Dictionary<Guid, List<Operation>>
        {
            { Guid.NewGuid(), _fixture.CreateMany<Operation>(2).ToList() }
        };

        _serviceMock
            .Setup(s => s.GroupOperationsByCategory(data))
            .Returns(expectedDict);

        // Перехватываем вывод в консоль
        using var writer = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(writer);

        // Act
        var result = _proxy.GroupOperationsByCategory(data);

        // Восстанавливаем консоль
        Console.SetOut(originalOut);

        // Assert
        Assert.Equal(expectedDict, result);

        // Проверяем, что метод был делегирован
        _serviceMock.Verify(s => s.GroupOperationsByCategory(data), Times.Once);

        // Проверяем, что логирование действительно было
        var output = writer.GetStringBuilder().ToString();
        Assert.Contains("Запущен метод GroupOperationsByCategory", output);
        Assert.Contains("Метод GroupOperationsByCategory завершен за", output);
    }
}