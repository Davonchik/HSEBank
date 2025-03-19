using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;

namespace HSEBank.BusinessLogic.Services.Facades;

/// <summary>
/// Implementation of Account Facade (CRUD logic).
/// </summary>
public class AccountFacade : IAccountFacade
{
    private IFinancialFactory _financialFactory;

    private IAccountRepository _accountRepository;
    
    public AccountFacade(IFinancialFactory financialFactory, IAccountRepository accountRepository)
    {
        _financialFactory = financialFactory;
        _accountRepository = accountRepository;
    }

    public BankAccount Create(BankAccountDto accountDto)
    {
        var newBankAccount = _financialFactory.CreateBankAccount(accountDto);
        _accountRepository.Create(newBankAccount);
        return newBankAccount;
    }

    public BankAccount GetById(Guid id)
    {
        if (!AccountExists(id))
        {
            throw new ArgumentException($"Аккаунта с таким ID'{id}' не существует!");
        }
        return _accountRepository.GetById(id);
    }

    public bool EditBankAccount(EditBankAccountDto accountDto)
    {
        if (!AccountExists(accountDto.BankAccountId))
        {
            throw new ArgumentException($"Аккаунта с таким ID'{accountDto.BankAccountId}' не существует!");
        }
        return _accountRepository.Update(accountDto);
    }

    public bool DeleteBankAccount(Guid bankAccountId)
    {
        if (!AccountExists(bankAccountId))
        {
            throw new ArgumentException($"Аккаунта с таким ID'{bankAccountId}' не существует!");
        }
        return _accountRepository.Delete(bankAccountId);
    }

    public IEnumerable<BankAccount> GetAllBankAccounts()
    {
        return _accountRepository.GetAll();
    }

    public bool AccountExists(Guid id)
    {
        return _accountRepository.Exists(id);
    }
}