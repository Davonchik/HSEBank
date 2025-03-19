using Microsoft.Extensions.DependencyInjection;
using HSEBank.BusinessLogic.Services;
using HSEBank.BusinessLogic.Services.Abstractions;
using HSEBank.BusinessLogic.Services.Facades;
using HSEBank.BusinessLogic.Dto;
using HSEBank.DataAccess.Models;
using HSEBank.DataAccess.Repositories;
using HSEBank.DataAccess.Repositories.Abstractions;
using HSEBank.Presentation;
using Type = HSEBank.DataAccess.Common.Enums.Type;

Directory.CreateDirectory("import");
Directory.CreateDirectory("export");

// DI SETTINGS.

var services = new ServiceCollection();

// Main Facades and Repositories.
services.AddScoped<IAccountFacade, AccountFacade>();
services.AddScoped<ICategoryFacade, CategoryFacade>();
services.AddScoped<IOperationFacade, OperationFacade>();

services.AddScoped<IAccountRepository, AccountRepository>();
services.AddScoped<ICategoryRepository, CategoryRepository>();
services.AddScoped<IOperationRepository, OperationRepository>();

// Factories.
services.AddScoped<IFinancialFactory, FinancialFactory>();

// Analytics service and decorator.
services.AddScoped<AnalyticsService>();
services.AddScoped<IAnalyticsService>(provider =>
    new AnalyticsServiceLoggerProxy(provider.GetRequiredService<AnalyticsService>()));

// FinancialFacade registration.
services.AddScoped<IFinancialFacade>(provider => FinancialFacade.GetInstance(
    provider.GetRequiredService<IAccountFacade>(),
    provider.GetRequiredService<ICategoryFacade>(),
    provider.GetRequiredService<IOperationFacade>(),
    provider.GetRequiredService<IAnalyticsService>()));

var serviceProvider = services.BuildServiceProvider();

// Get Facade from DI.
var financialFacade = serviceProvider.GetRequiredService<IFinancialFacade>();

static bool CheckImportPath(string importFileName, out string importFilePath)
{
    importFilePath = Path.Combine(Environment.CurrentDirectory, "import", importFileName);
    if (!File.Exists(importFilePath))
    {
        Console.WriteLine("Такого файла в папке import не существует!");
        return false;
    }
    return true;
}

static bool CheckExportDirectory(string exportFileName, out string exportFilePath)
{
    exportFilePath = Path.Combine(Environment.CurrentDirectory, "export");
    if (!Directory.Exists(exportFilePath))
    {
        Console.WriteLine("Папки export не существует!");
        return false;
    }
    exportFilePath = Path.Combine(exportFilePath, exportFileName);
    return true;
}

// Menu for Import / Export logic.
static void ProcessImportExportMenu(IFinancialFacade facade)
{
    Console.Write("Введите имя файла: ");
    string fileName = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(fileName))
    {
        Console.WriteLine("Вы ввели пустое имя файла!");
        return;
    } 
    
    Console.WriteLine("\nВыберите действие импорта/экспорта:");
    Console.WriteLine("1. Импорт операций");
    Console.WriteLine("2. Экспорт операций");
    Console.WriteLine("3. Импорт банковских счетов");
    Console.WriteLine("4. Экспорт банковских счетов");
    Console.WriteLine("5. Импорт категорий");
    Console.WriteLine("6. Экспорт категорий");
    Console.Write("Ваш выбор: ");
    var ioChoice = Console.ReadLine();

    string filePath = String.Empty;
    switch (ioChoice)
    {
        case "1":
            if (!CheckImportPath(fileName, out filePath)) break;
            facade.ImportOperationsFromFile(filePath);
            Console.WriteLine("Операции импортированы.");
            break;
        case "2":
            if (!CheckExportDirectory(fileName, out filePath)) break;
            facade.ExportOperationsFromFile(filePath);
            Console.WriteLine("Операции экспортированы.");
            break;
        case "3":
            if (!CheckImportPath(fileName, out filePath)) break;
            facade.ImportBankAccountsFromFile(filePath);
            Console.WriteLine("Банковские счета импортированы.");
            break;
        case "4":
            if (!CheckExportDirectory(fileName, out filePath)) break;
            facade.ExportBankAccountsFromFile(filePath);
            Console.WriteLine("Банковские счета экспортированы.");
            break;
        case "5":
            if (!CheckImportPath(fileName, out filePath)) break;
            facade.ImportCategoriesFromFile(filePath);
            Console.WriteLine("Категории импортированы.");
            break;
        case "6":
            if (!CheckExportDirectory(fileName, out filePath)) break;
            facade.ExportCategoriesFromFile(filePath);
            Console.WriteLine("Категории экспортированы.");
            break;
        default:
            Console.WriteLine("Неверный выбор в подменю.");
            break;
    }
}

// Main part - Console app.

bool exitRequested = false;

while (!exitRequested)
{
    Console.WriteLine("\nВыберите действие:");
    Console.WriteLine("1. Создать банковский счёт");
    Console.WriteLine("2. Создать категорию");
    Console.WriteLine("3. Создать операцию");
    Console.WriteLine("4. Пересчитать баланс счёта");
    Console.WriteLine("5. Показать все операции");
    Console.WriteLine("6. Показать все категории");
    Console.WriteLine("7. Показать все счета");
    Console.WriteLine("8. Получить разницу доходов и расходов за период");
    Console.WriteLine("9. Группировка операций по категориям");
    Console.WriteLine("10. Редактировать счёт");
    Console.WriteLine("11. Удалить счёт");
    Console.WriteLine("12. Редактировать категорию");
    Console.WriteLine("13. Удалить категорию");
    Console.WriteLine("14. Редактировать операцию");
    Console.WriteLine("15. Удалить операцию");
    Console.WriteLine("16. Импорт/Экспорт данных");
    Console.WriteLine("17. Выход");

    try
    {
        switch (Console.ReadLine())
        {
            case "1":
                Console.Write("Введите название счёта: ");
                var accountName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(accountName))
                {
                    Console.WriteLine("Вы ввели пустое имя аккаунта!");
                    break;
                }
                
                var newAccount = financialFacade.CreateBankAccount(new BankAccountDto
                {
                    Name = accountName,
                    Balance = 0
                });

                Console.WriteLine($"Создан счёт: {newAccount.Id}, {newAccount.Name}");
                break;

            case "2":
                Console.Write("Введите название категории: ");
                var categoryName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    Console.WriteLine("Вы ввели пустое имя категории!");
                    break;
                }
                
                Console.Write("Введите тип (1 - если Income; 2 - если Expense): ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out int choice) || (choice != 1 && choice != 2))
                {
                    Console.WriteLine("Некорректный выбор типа. Введите 1 для Income или 2 для Expense.");
                    break;
                }

                var type = (choice == 1) ? Type.Income : Type.Expense;


                var newCategory = financialFacade.CreateCategory(new CategoryDto
                {
                    Name = categoryName,
                    Type = type
                });

                Console.WriteLine($"Создана категория: {newCategory.CategoryId}, {newCategory.Name}");
                break;

            case "3":
                Console.Write("Введите ID счёта: ");
                if (!Guid.TryParse(Console.ReadLine(), out Guid accountId))
                {
                    Console.WriteLine("Некорректный ID счёта!");
                    break;
                }

                Console.Write("Введите ID категории: ");
                if (!Guid.TryParse(Console.ReadLine(), out Guid categoryId))
                {
                    Console.WriteLine("Некорректный ID категории!");
                    break;
                }

                Console.Write("Введите сумму операции: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    Console.WriteLine("Некорректная сумма!");
                    break;
                }

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
                //var balanceAccountId = Guid.Parse(Console.ReadLine());
                if (!Guid.TryParse(Console.ReadLine(), out Guid balanceAccountId))
                {
                    Console.WriteLine("Некорректный ID счёта!");
                    break;
                }
                var recalculatedBalance = financialFacade.RecalculateBalance(balanceAccountId);

                Console.WriteLine($"Пересчитанный баланс: {recalculatedBalance}");
                break;

            case "5":
                var operations = financialFacade.GetAllOperations().ToList();
                if (!operations.Any())
                {
                    Console.WriteLine("Пока операций не существует!");
                    break;
                }

                foreach (var op in operations)
                {
                    Console.WriteLine($"Операция {op.Id}: {op.Amount}, {op.Type}, Дата: {op.Date}");
                }
                break;
            
            case "6":
                var categories = financialFacade.GetAllCategories().ToList();
                if (!categories.Any())
                {
                    Console.WriteLine("Пока категорий не существует!");
                    break;
                }

                foreach (var category in categories)
                {
                    Console.WriteLine($"Категория {category.CategoryId} --- {category.Name}, тип: {category.Type}");
                }
                break;
            
            case "7":
                var allBankAccounts = financialFacade.GetAllBankAccounts().ToList();
                if (!allBankAccounts.Any())
                {
                    Console.WriteLine("Ещё нет ни одного счёта!");
                    break;
                }
                foreach (var ba in allBankAccounts)
                {
                    Console.WriteLine($"Bank Account: {ba.Id}, Name: {ba.Name}, Balance: {ba.Balance}");
                }
                break;

            case "8":
                Console.WriteLine("Введите начальную дату (yyyy-mm-dd hh:mm:ss): ");
                //var startDate = DateTime.Parse(Console.ReadLine());
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                {
                    Console.WriteLine("Некорректный формат даты!");
                    break;
                }
                Console.WriteLine("Введите конечную дату (yyyy-mm-dd hh:mm:ss):");
                //var endDate = DateTime.Parse(Console.ReadLine());
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                {
                    Console.WriteLine("Некорректный формат даты!");
                    break;
                }

                var data = new FinancialDataDto
                {
                    Operations = financialFacade.GetAllOperations().ToList()
                };

                var balanceDifference = financialFacade.GetBalanceDifference(data, startDate, endDate);

                Console.WriteLine($"Разница доходов и расходов: {balanceDifference}");
                break;

            case "9":
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

            case "10":
                Console.Write("Введите ID счёта для редактирования: ");
                if (!Guid.TryParse(Console.ReadLine(), out Guid newAccountId))
                {
                    Console.WriteLine("Некорректный ID счёта!");
                    break;
                }
                
                Console.Write("Введите новое название для счёта: ");
                var newAccountName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newAccountName))
                {
                    Console.WriteLine("Вы ввели пустое имя аккаунта!");
                    break;
                }

                var editBankAccountDto = new EditBankAccountDto
                {
                    BankAccountId = newAccountId,
                    Name = newAccountName,
                };

                financialFacade.EditBankAccount(editBankAccountDto);
                break;

            case "11":
                Console.Write("Введите ID счёта для удаления: ");
                if (!Guid.TryParse(Console.ReadLine(), out Guid deleteAccountId))
                {
                    Console.WriteLine("Некорректный ID счёта!");
                    break;
                }
                
                financialFacade.DeleteBankAccount(deleteAccountId);
                break;
                
            case "12":
                Console.Write("Введите ID категории для редактирования: ");
                if (!Guid.TryParse(Console.ReadLine(), out Guid newCategoryId))
                {
                    Console.WriteLine("Некорректный ID категории!");
                    break;
                }
                
                Console.Write("Введите новое название для категории: ");
                var newCategoryName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newCategoryName))
                {
                    Console.WriteLine("Вы ввели пустое имя категории!");
                    break;
                }

                var editCategoryDto = new EditCategoryDto
                {
                    CategoryId = newCategoryId,
                    Name = newCategoryName,
                };

                financialFacade.EditCategory(editCategoryDto);
                break;
            
            case "13":
                Console.Write("Введите ID категории для удаления: ");
                if (!Guid.TryParse(Console.ReadLine(), out Guid deleteCategoryId))
                {
                    Console.WriteLine("Некорректный ID категории!");
                    break;
                }

                financialFacade.DeleteCategory(deleteCategoryId);
                break;
            
            case "14":
                Console.Write("Введите ID операции, в которой хотите поменять категорию: ");
                if (!Guid.TryParse(Console.ReadLine(), out Guid newOperationId))
                {
                    Console.WriteLine("Некорректный ID операции!");
                    break;
                }
                
                Console.Write("Введите ID категории: ");
                if (!Guid.TryParse(Console.ReadLine(), out Guid newOpCategoryId))
                {
                    Console.WriteLine("Некорректный ID категории!");
                    break;
                }

                var editOperationDto = new EditOperationDto
                {
                    OperationId = newOperationId,
                    CategoryId = newOpCategoryId,
                };
                
                financialFacade.EditOperation(editOperationDto);
                break;
            
            case "15":
                Console.Write("Введите ID операции для удаления: ");
                if (!Guid.TryParse(Console.ReadLine(), out Guid deleteOperationId))
                {
                    Console.WriteLine("Некорректный ID операции!");
                    break;
                }

                financialFacade.DeleteOperation(deleteOperationId);
                break;
            
            case "16":
                ProcessImportExportMenu(financialFacade);
                break;
            
            case "17":
                exitRequested = true;
                break;

            default:
                Console.WriteLine("Неверный выбор. Попробуйте снова.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

Console.WriteLine("Приложение завершило работу.");
