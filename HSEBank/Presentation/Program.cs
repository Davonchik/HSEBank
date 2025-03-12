using Microsoft.Extensions.DependencyInjection;
using HSEBank.BusinessLogic.Services;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Repositories;
using HSEBank.DataAccess.Repositories.Abstractions;
using HSEBank.Presentation;
using Type = HSEBank.DataAccess.Common.Enums.Type;

var services = new ServiceCollection();

// Основные фасады и репозитории
services.AddScoped<IAccountFacade, AccountFacade>();
services.AddScoped<ICategoryFacade, CategoryFacade>();
services.AddScoped<IOperationFacade, OperationFacade>();

services.AddScoped<IAccountRepository, AccountRepository>();
services.AddScoped<ICategoryRepository, CategoryRepository>();
services.AddScoped<IOperationRepository, OperationRepository>();

// Фабрики
services.AddScoped<IFinancialFactory, FinancialFactory>();

// Сервис аналитики и декоратор
services.AddScoped<AnalyticsService>();
services.AddScoped<IAnalyticsService>(provider =>
    new AnalyticsServiceLoggerProxy(provider.GetRequiredService<AnalyticsService>()));

// Регистрация главного фасада (FinancialFacade)
services.AddScoped<IFinancialFacade>(provider => FinancialFacade.GetInstance(
    provider.GetRequiredService<IAccountFacade>(),
    provider.GetRequiredService<ICategoryFacade>(),
    provider.GetRequiredService<IOperationFacade>(),
    provider.GetRequiredService<IAnalyticsService>()));


var serviceProvider = services.BuildServiceProvider();

// Получаем фасад из DI
var financialFacade = serviceProvider.GetRequiredService<IFinancialFacade>();

bool exitRequested = false;

while (!exitRequested)
{
    Console.WriteLine("\nВыберите действие:");
    Console.WriteLine("1. Создать банковский счет");
    Console.WriteLine("2. Создать категорию");
    Console.WriteLine("3. Создать операцию");
    Console.WriteLine("4. Пересчитать баланс счёта");
    Console.WriteLine("5. Показать все операции");
    Console.WriteLine("6. Получить разницу доходов и расходов за период");
    Console.WriteLine("7. Группировка операций по категориям");
    Console.WriteLine("8. Выход");

    switch (Console.ReadLine())
    {
        case "1":
            Console.Write("Введите название счёта: ");
            var accountName = Console.ReadLine();

            var newAccount = financialFacade.CreateBankAccount(new BankAccountDto
            {
                Name = accountName,
                InitBalance = 0
            });

            Console.WriteLine($"Создан счёт: {newAccount.Id}, {newAccount.Name}");
            break;

        case "2":
            Console.Write("Введите название категории: ");
            var categoryName = Console.ReadLine();
            Console.Write("Введите тип (Income или Expense): ");
            var type = Enum.Parse<Type>(Console.ReadLine(), true);

            var newCategory = financialFacade.CreateCategory(new CategoryDto
            {
                Name = categoryName,
                Type = type
            });

            Console.WriteLine($"Создана категория: {newCategory.Id}, {newCategory.Name}");
            break;

        case "3":
            Console.Write("Введите ID счёта: ");
            var accountId = Guid.Parse(Console.ReadLine());

            Console.Write("Введите ID категории: ");
            var categoryId = Guid.Parse(Console.ReadLine());

            Console.Write("Введите сумму операции: ");
            var amount = decimal.Parse(Console.ReadLine());

            var operation = financialFacade.CreateOperation(new OperationDto
            {
                BankAccountId = accountId,
                CategoryId = categoryId,
                Amount = amount,
            });

            Console.WriteLine($"Создана операция: {operation.Id}");
            break;

        case "4":
            Console.Write("Введите ID счёта для пересчёта: ");
            var balanceAccountId = Guid.Parse(Console.ReadLine());
            var recalculatedBalance = financialFacade.RecalculateBalance(balanceAccountId);

            Console.WriteLine($"Пересчитанный баланс: {recalculatedBalance}");
            break;

        case "5":
            var operations = financialFacade.GetAllOperations();
            foreach (var op in operations)
            {
                Console.WriteLine($"Операция {op.Id}: {op.Amount}, {op.Type}, Дата: {op.Date}");
            }
            break;
        
        case "6":
            Console.WriteLine("Введите начальную дату (yyyy-mm-dd): ");
            var startDate = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Введите конечную дату (yyyy-mm-dd):");
            var endDate = DateTime.Parse(Console.ReadLine());

            var data = new FinancialDataDto
            {
                Operations = financialFacade.GetAllOperations().ToList()
            };

            var balanceDifference = financialFacade.GetBalanceDifference(data, startDate, endDate);

            Console.WriteLine($"Разница доходов и расходов: {balanceDifference}");
            break;

        case "7":
            var financialData = new FinancialDataDto
            {
                Operations = financialFacade.GetAllOperations().ToList()
            };

            var groupedOperations = financialFacade.GroupOperationsByCategory(financialData);

            foreach (var categoryGroup in groupedOperations)
            {
                var category = financialFacade.GetCategory(categoryGroup.Key);
                Console.WriteLine($"\nКатегория: {category.Name}");

                foreach (var op in categoryGroup.Value)
                {
                    Console.WriteLine($"Операция: {op.Id}, Сумма: {op.Amount}, Дата: {op.Date}");
                }
            }
            break;

        case "8":
            exitRequested = true;
            break;

        default:
            Console.WriteLine("Неверный выбор. Попробуйте снова.");
            break;
    }
}

Console.WriteLine("Приложение завершило работу.");
