using AutoFixture;
using HSEBank.BusinessLogic.Dto;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.BusinessLogic.Shared;
using HSEBank.Presentation;
using Moq;

namespace TestHSEBank;

public class FinancialFacadeTests
{
    private Mock<IAccountFacade> _accountMock;
    private Mock<ICategoryFacade> _categoryMock;
    private Mock<IOperationFacade> _operationMock;
    
    private readonly Fixture _fixture;
    
    public FinancialFacadeTests()
    {
        _accountMock = new Mock<IAccountFacade>();
        _categoryMock = new Mock<ICategoryFacade>();
        _operationMock = new Mock<IOperationFacade>();
        _fixture = new Fixture();
    }
    
    [Fact]
    public void Throw_Exception_If_No_Such_Category()
    {
        _categoryMock.Setup(facade => facade.CategoryExists(It.IsAny<Guid>())).Returns(false);
        _accountMock.Setup(facade => facade.AccountExists(It.IsAny<Guid>())).Returns(true);
        FinancialFacade financialFacade = FinancialFacade.GetInstance(_accountMock.Object, _categoryMock.Object, _operationMock.Object);
        OperationDto operationDto = _fixture.Build<OperationDto>()
            .With(dto => dto.Type, OperationType.Expense)
            .Create<OperationDto>();
        
        Assert.Throws<ArgumentException>(() => financialFacade.CreateOperation(operationDto));
    }
}