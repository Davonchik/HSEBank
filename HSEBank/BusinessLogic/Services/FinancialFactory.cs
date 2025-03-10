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
            throw new ArgumentException("Amount cannot be negative");
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
        return new BankAccount
        {
            Id = Guid.NewGuid(),
            Name = bankAccountDto.Name,
            Balance = bankAccountDto.InitBalance
        };
    }

    public Category CreateCategory(CategoryDto categoryDto)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = categoryDto.Name,
            Type = categoryDto.Type
        };
    }
}