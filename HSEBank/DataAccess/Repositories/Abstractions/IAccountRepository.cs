using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;

namespace HSEBank.DataAccess.Repositories.Abstractions;

/// <summary>
/// Service for working with accounts.
/// </summary>
public interface IAccountRepository : IRepository<BankAccount, EditBankAccountDto>;