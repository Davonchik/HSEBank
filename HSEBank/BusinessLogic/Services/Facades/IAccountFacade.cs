using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.DataAccess.Models;

namespace HSEBank.BusinessLogic.Services.Facades;

public interface IAccountFacade
{
    public BankAccount Create(BankAccountDto accountDto);
    
    public BankAccount GetById(Guid id);
    
    public bool EditBankAccount(EditBankAccountDto accountDto);
    
    public bool DeleteBankAccount(Guid bankAccountId);
    
    public IEnumerable<BankAccount> GetAllBankAccounts();
}