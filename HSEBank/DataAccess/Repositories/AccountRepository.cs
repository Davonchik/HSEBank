using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;

namespace HSEBank.DataAccess.Repositories;

/// <summary>
/// Bank Account service.
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly Dictionary<Guid, BankAccount> _accounts = new();
    
    public BankAccount Create(BankAccount account)
    {
        _accounts[account.Id] = account;
        return account;
    }

    public BankAccount GetById(Guid id)
    {
        return _accounts.TryGetValue(id, out var acc) ? acc : throw new KeyNotFoundException("Счёт не найден");
    }

    public bool Update(EditBankAccountDto dto)
    {
        if (_accounts.ContainsKey(dto.BankAccountId))
        {
            _accounts[dto.BankAccountId].Name = dto.Name;
            return true;
        }
        
        return false;
    }

    public bool Delete(Guid id)
    {
        return _accounts.Remove(id);
    }

    public IEnumerable<BankAccount> GetAll()
    {
        return _accounts.Values;
    }

    public IEnumerable<BankAccount> GetByCondition(Func<BankAccount, bool> predicate)
    {
        return _accounts.Values.Where(predicate);
    }

    public bool Exists(Guid id)
    {
        return _accounts.ContainsKey(id);
    }
}