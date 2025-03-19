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
        var expectedAccount = _fixture.Create<BankAccount>();

        _financialFactoryMock.Setup(f => f.CreateBankAccount(bankAccountDto))
                             .Returns(expectedAccount);
        _accountRepositoryMock.Setup(r => r.Create(expectedAccount))
                              .Returns(expectedAccount);

        // Act
        var result = _accountFacade.Create(bankAccountDto);

        // Assert
        Assert.Equal(expectedAccount, result);
        _financialFactoryMock.Verify(f => f.CreateBankAccount(bankAccountDto), Times.Once);
        _accountRepositoryMock.Verify(r => r.Create(expectedAccount), Times.Once);
    }

    [Fact]
    public void GetById_Should_Throw_Exception_If_Account_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _accountRepositoryMock.Setup(r => r.Exists(id)).Returns(false);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _accountFacade.GetById(id));
        Assert.Contains(id.ToString(), ex.Message);
        Assert.Contains("не существует", ex.Message);
    }

    [Fact]
    public void GetById_Should_Return_BankAccount_If_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedAccount = _fixture.Create<BankAccount>();
        _accountRepositoryMock.Setup(r => r.Exists(id)).Returns(true);
        _accountRepositoryMock.Setup(r => r.GetById(id)).Returns(expectedAccount);

        // Act
        var result = _accountFacade.GetById(id);

        // Assert
        Assert.Equal(expectedAccount, result);
    }

    [Fact]
    public void EditBankAccount_Should_Throw_Exception_If_Account_Does_Not_Exist()
    {
        // Arrange
        var editDto = _fixture.Create<EditBankAccountDto>();
        _accountRepositoryMock.Setup(r => r.Exists(editDto.BankAccountId)).Returns(false);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _accountFacade.EditBankAccount(editDto));
        Assert.Contains(editDto.BankAccountId.ToString(), ex.Message);
        Assert.Contains("не существует", ex.Message);
    }

    [Fact]
    public void EditBankAccount_Should_Call_Update_And_Return_True_When_Account_Exists()
    {
        // Arrange
        var editDto = _fixture.Create<EditBankAccountDto>();
        _accountRepositoryMock.Setup(r => r.Exists(editDto.BankAccountId)).Returns(true);
        _accountRepositoryMock.Setup(r => r.Update(editDto)).Returns(true);

        // Act
        var result = _accountFacade.EditBankAccount(editDto);

        // Assert
        Assert.True(result);
        _accountRepositoryMock.Verify(r => r.Update(editDto), Times.Once);
    }

    [Fact]
    public void DeleteBankAccount_Should_Throw_Exception_If_Account_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _accountRepositoryMock.Setup(r => r.Exists(id)).Returns(false);

        // Act & Assert: проверяем, что выбрасывается исключение и сообщение содержит идентификатор и фразу "не существует"
        var ex = Assert.Throws<ArgumentException>(() => _accountFacade.DeleteBankAccount(id));
        Assert.Contains(id.ToString(), ex.Message);
        Assert.Contains("не существует", ex.Message);
    }

    [Fact]
    public void DeleteBankAccount_Should_Call_Delete_And_Return_True_When_Account_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _accountRepositoryMock.Setup(r => r.Exists(id)).Returns(true);
        _accountRepositoryMock.Setup(r => r.Delete(id)).Returns(true);

        // Act
        var result = _accountFacade.DeleteBankAccount(id);

        // Assert
        Assert.True(result);
        _accountRepositoryMock.Verify(r => r.Delete(id), Times.Once);
    }

    [Fact]
    public void GetAllBankAccounts_Should_Return_All_BankAccounts()
    {
        // Arrange
        var accounts = _fixture.CreateMany<BankAccount>(3);
        _accountRepositoryMock.Setup(r => r.GetAll()).Returns(accounts);

        // Act
        var result = _accountFacade.GetAllBankAccounts();

        // Assert
        Assert.Equal(accounts, result);
    }

    [Fact]
    public void AccountExists_Should_Return_True_If_Account_Exists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _accountRepositoryMock.Setup(r => r.Exists(id)).Returns(true);

        // Act
        var result = _accountFacade.AccountExists(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AccountExists_Should_Return_False_If_Account_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _accountRepositoryMock.Setup(r => r.Exists(id)).Returns(false);

        // Act
        var result = _accountFacade.AccountExists(id);

        // Assert
        Assert.False(result);
    }
}