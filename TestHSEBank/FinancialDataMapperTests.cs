using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Mappers;
using HSEBank.DataAccess.Models;
using Type = HSEBank.DataAccess.Common.Enums.Type;

namespace TestHSEBank;

public class FinancialDataMapperTests
{
    [Fact]
    public void Map_ShouldReturn_FinancialDataDto_With_PopulatedLists()
    {
        // Arrange
        var bankAccounts = new List<BankAccount>
        {
            new BankAccount { Id = Guid.NewGuid(), Name = "Account1", Balance = 100 },
            new BankAccount { Id = Guid.NewGuid(), Name = "Account2", Balance = 200 }
        };

        var categories = new List<Category>
        {
            new Category { CategoryId = Guid.NewGuid(), Name = "Category1", Type = Type.Income },
            new Category { CategoryId = Guid.NewGuid(), Name = "Category2", Type = Type.Expense }
        };

        var operations = new List<Operation>
        {
            new Operation { Id = Guid.NewGuid(), Amount = 50, BankAccountId = bankAccounts[0].Id, CategoryId = categories[0].CategoryId, Date = DateTime.Now, Type = Type.Income },
            new Operation { Id = Guid.NewGuid(), Amount = 75, BankAccountId = bankAccounts[1].Id, CategoryId = categories[1].CategoryId, Date = DateTime.Now, Type = Type.Expense }
        };

        // Act
        FinancialDataDto result = FinancialDataMapper.Map(bankAccounts, categories, operations);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bankAccounts.Count, result.BankAccounts.Count);
        Assert.Equal(categories.Count, result.Categories.Count);
        Assert.Equal(operations.Count, result.Operations.Count);
        
        for (int i = 0; i < bankAccounts.Count; i++)
        {
            Assert.Equal(bankAccounts[i].Id, result.BankAccounts[i].Id);
            Assert.Equal(bankAccounts[i].Name, result.BankAccounts[i].Name);
            Assert.Equal(bankAccounts[i].Balance, result.BankAccounts[i].Balance);
        }
        for (int i = 0; i < categories.Count; i++)
        {
            Assert.Equal(categories[i].CategoryId, result.Categories[i].CategoryId);
            Assert.Equal(categories[i].Name, result.Categories[i].Name);
            Assert.Equal(categories[i].Type, result.Categories[i].Type);
        }
        for (int i = 0; i < operations.Count; i++)
        {
            Assert.Equal(operations[i].Id, result.Operations[i].Id);
            Assert.Equal(operations[i].Amount, result.Operations[i].Amount);
            Assert.Equal(operations[i].BankAccountId, result.Operations[i].BankAccountId);
            Assert.Equal(operations[i].CategoryId, result.Operations[i].CategoryId);
            Assert.Equal(operations[i].Type, result.Operations[i].Type);
        }
    }

    [Fact]
    public void Map_ShouldReturn_FinancialDataDto_With_EmptyLists_WhenInputsAreEmpty()
    {
        // Arrange
        var bankAccounts = Enumerable.Empty<BankAccount>();
        var categories = Enumerable.Empty<Category>();
        var operations = Enumerable.Empty<Operation>();

        // Act
        FinancialDataDto result = FinancialDataMapper.Map(bankAccounts, categories, operations);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.BankAccounts);
        Assert.Empty(result.Categories);
        Assert.Empty(result.Operations);
    }
}