using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories.Abstractions;
using Moq;

namespace TestHSEBank;

public class AccountFacadeTests
{
    private readonly Mock<IFinancialFactory> _financialFactoryMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Fixture _fixture;
    private readonly AccountFacade _accountFacade;

    public AccountFacadeTests()
    {
        _financialFactoryMock = new Mock<IFinancialFactory>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _fixture = new Fixture();
        _accountFacade = new AccountFacade(_financialFactoryMock.Object, _accountRepositoryMock.Object);
    }

    [Fact]
    public void Create_Should_Call_FinancialFactory_And_Repository_And_Return_BankAccount()
    {
        // Arrange
        var bankAccountDto = _fixture.Create<BankAccountDto>();
        var newBankAccount = _fixture.Create<BankAccount>();

        _financialFactoryMock
            .Setup(ff => ff.CreateBankAccount(bankAccountDto))
            .Returns(newBankAccount);

        // Act
        var result = _accountFacade.Create(bankAccountDto);

        // Assert
        Assert.Equal(newBankAccount, result);
        _financialFactoryMock.Verify(ff => ff.CreateBankAccount(bankAccountDto), Times.Once);
        _accountRepositoryMock.Verify(ar => ar.Create(newBankAccount), Times.Once);
    }

    [Fact]
    public void GetById_Should_Return_BankAccount_From_Repository()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedBankAccount = _fixture.Create<BankAccount>();

        _accountRepositoryMock
            .Setup(ar => ar.GetById(accountId))
            .Returns(expectedBankAccount);

        // Act
        var result = _accountFacade.GetById(accountId);

        // Assert
        Assert.Equal(expectedBankAccount, result);
    }

    [Fact]
    public void EditBankAccount_Should_Call_Repository_Update_And_Return_Result()
    {
        // Arrange
        var editBankAccountDto = _fixture.Create<EditBankAccountDto>();
        _accountRepositoryMock
            .Setup(ar => ar.Update(editBankAccountDto))
            .Returns(true);

        // Act
        var result = _accountFacade.EditBankAccount(editBankAccountDto);

        // Assert
        Assert.True(result);
        _accountRepositoryMock.Verify(ar => ar.Update(editBankAccountDto), Times.Once);
    }

    [Fact]
    public void DeleteBankAccount_Should_Call_Repository_Delete_And_Return_Result()
    {
        // Arrange
        var bankAccountId = Guid.NewGuid();
        _accountRepositoryMock
            .Setup(ar => ar.Delete(bankAccountId))
            .Returns(true);

        // Act
        var result = _accountFacade.DeleteBankAccount(bankAccountId);

        // Assert
        Assert.True(result);
        _accountRepositoryMock.Verify(ar => ar.Delete(bankAccountId), Times.Once);
    }

    [Fact]
    public void GetAllBankAccounts_Should_Return_All_BankAccounts_From_Repository()
    {
        // Arrange
        var bankAccounts = _fixture.CreateMany<BankAccount>(3);
        _accountRepositoryMock
            .Setup(ar => ar.GetAll())
            .Returns(bankAccounts);

        // Act
        var result = _accountFacade.GetAllBankAccounts();

        // Assert
        Assert.Equal(bankAccounts, result);
    }

    [Fact]
    public void AccountExists_Should_Return_Repository_Result()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        _accountRepositoryMock
            .Setup(ar => ar.Exists(accountId))
            .Returns(true);

        // Act
        var result = _accountFacade.AccountExists(accountId);

        // Assert
        Assert.True(result);
    }
}