using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories;

namespace TestHSEBank;

public class AccountRepositoryTests
{
    private readonly AccountRepository _repository;
    private readonly Fixture _fixture;

    public AccountRepositoryTests()
    {
        _repository = new AccountRepository();
        _fixture = new Fixture();
    }

    [Fact]
    public void Create_Should_Add_Account_To_Repository()
    {
        // Arrange
        var account = _fixture.Create<BankAccount>();

        // Act
        var createdAccount = _repository.Create(account);

        // Assert
        Assert.Equal(account, createdAccount);
        Assert.True(_repository.Exists(account.Id));
    }

    [Fact]
    public void GetById_Should_Return_Account_When_It_Exists()
    {
        // Arrange
        var account = _fixture.Create<BankAccount>();
        _repository.Create(account);

        // Act
        var retrievedAccount = _repository.GetById(account.Id);

        // Assert
        Assert.Equal(account, retrievedAccount);
    }

    [Fact]
    public void GetById_Should_Throw_KeyNotFoundException_When_Account_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => _repository.GetById(id));
        Assert.Equal("Счёт не найден", exception.Message);
    }

    [Fact]
    public void Update_Should_Return_True_And_Update_Account_When_Account_Exists()
    {
        // Arrange
        var account = _fixture.Create<BankAccount>();
        _repository.Create(account);
        var newName = "Updated Name";

        var editDto = _fixture.Build<EditBankAccountDto>()
            .With(dto => dto.BankAccountId, account.Id)
            .With(dto => dto.Name, newName)
            .Create();

        // Act
        var result = _repository.Update(editDto);

        // Assert
        Assert.True(result);
        var updatedAccount = _repository.GetById(account.Id);
        Assert.Equal(newName, updatedAccount.Name);
    }

    [Fact]
    public void Update_Should_Return_False_When_Account_Does_Not_Exist()
    {
        // Arrange
        var editDto = _fixture.Create<EditBankAccountDto>();

        // Act
        var result = _repository.Update(editDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Delete_Should_Remove_Account_And_Return_True_When_Account_Exists()
    {
        // Arrange
        var account = _fixture.Create<BankAccount>();
        _repository.Create(account);

        // Act
        var result = _repository.Delete(account.Id);

        // Assert
        Assert.True(result);
        Assert.False(_repository.Exists(account.Id));
    }

    [Fact]
    public void Delete_Should_Return_False_When_Account_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = _repository.Delete(id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetAll_Should_Return_All_Accounts()
    {
        // Arrange
        var accounts = _fixture.CreateMany<BankAccount>(3).ToList();
        foreach (var account in accounts)
        {
            _repository.Create(account);
        }

        // Act
        var allAccounts = _repository.GetAll().ToList();

        // Assert
        Assert.Equal(accounts.Count, allAccounts.Count);
        foreach (var account in accounts)
        {
            Assert.Contains(account, allAccounts);
        }
    }

    [Fact]
    public void GetByCondition_Should_Return_Filtered_Accounts()
    {
        // Arrange
        var accounts = _fixture.CreateMany<BankAccount>(5).ToList();
        // Сделаем имя одной учётной записи уникальным для фильтрации
        var targetAccount = accounts.First();
        targetAccount.Name = "Target";
        foreach (var account in accounts)
        {
            _repository.Create(account);
        }

        // Act
        var filteredAccounts = _repository.GetByCondition(a => a.Name == "Target").ToList();

        // Assert
        Assert.Single(filteredAccounts);
        Assert.Equal("Target", filteredAccounts.First().Name);
    }

    [Fact]
    public void Exists_Should_Return_True_When_Account_Exists_And_False_Otherwise()
    {
        // Arrange
        var account = _fixture.Create<BankAccount>();
        _repository.Create(account);

        // Act & Assert
        Assert.True(_repository.Exists(account.Id));
        Assert.False(_repository.Exists(Guid.NewGuid()));
    }
}