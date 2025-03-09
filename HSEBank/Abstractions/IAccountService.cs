using HSEBank.Dto;
using HSEBank.Models;

namespace HSEBank.Abstractions;

/// <summary>
/// Service for working with bank accounts.
/// </summary>
public interface IAccountService
{
    BankAccount Create(BankAccountDto dto);
    BankAccount GetById(Guid id);
    bool Update(EditBankAccountDto editDto);
    bool Delete(Guid id);
    IEnumerable<BankAccount> GetAll();
    IEnumerable<BankAccount> GetByCondition(Func<BankAccount, bool> predicate);
}