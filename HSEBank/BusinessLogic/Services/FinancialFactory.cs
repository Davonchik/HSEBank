using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Services;

/// <summary>
/// Factory for creating objects with centralized validation.
/// </summary>
public class FinancialFactory : IFinancialFactory
{
    public Operation CreateOperation(OperationDto operationDto)
    {
        if (operationDto.Amount < 0)
        {
            throw new ArgumentException("Стоимость не может быть отрицательной!");
        }

        return new Operation()
        {
            Amount = operationDto.Amount,
            Description = operationDto.Description,
            Type = operationDto.Type,
            BankAccountId = operationDto.BankAccountId,
            CategoryId = operationDto.CategoryId,
            Date = DateTime.Now,
            Id = Guid.NewGuid(),
        };
    }

    public BankAccount CreateBankAccount(BankAccountDto bankAccountDto)
    {
        if (bankAccountDto.Balance < 0)
        {
            throw new ArgumentException("Нельзя создать аккаунт с отрицательным балансом!");
        }
        return new BankAccount
        {
            Id = Guid.NewGuid(),
            Name = bankAccountDto.Name,
            Balance = bankAccountDto.Balance
        };
    }

    public Category CreateCategory(CategoryDto categoryDto)
    {
        var category = new Category()
        {
            Name = categoryDto.Name,
            Type = categoryDto.Type
        };

        if (categoryDto.CategoryId.HasValue)
        {
            category.CategoryId = categoryDto.CategoryId.Value;
            return category;
        }
        
        category.CategoryId = Guid.NewGuid();
        return category;
    }
}