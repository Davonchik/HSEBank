using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;

namespace HSEBank.BusinessLogic.Services.Facades;

public class AccountFacade : IAccountFacade
{
    private IFinancialFactory _financialFactory { get; }
    
    private IAccountRepository _accountRepository { get; }
    
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
        return _accountRepository.GetById(id);
    }

    public bool EditBankAccount(EditBankAccountDto accountDto)
    {
        return _accountRepository.Update(accountDto);
    }

    public bool DeleteBankAccount(Guid bankAccountId)
    {
        return _accountRepository.Delete(bankAccountId);
    }

    public IEnumerable<BankAccount> GetAllBankAccounts()
    {
        return _accountRepository.GetAll();
    }
}