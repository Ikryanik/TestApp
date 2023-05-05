using System.Diagnostics;
using TestApp;

var stopWatch = new Stopwatch();

var isCommand = int.TryParse(args[0], out var commandNumber);
if (!isCommand) return;

switch (commandNumber)
{
    case 1:
        await DbService.CreateTable();
        break;
    case 2:
        await CreateEntry(args);
        break;
    case 3:
        await ShowTable();
        break;
    case 4:
        await DbService.FillTheTable();
        break;
    case 5:
        await ShowEntriesWithFilter(stopWatch);
        break;
    case 6:
        await DbService.AddIndexes();
        break;
    default:
        Console.WriteLine("Введена некорректная команда");
        return;
}

async Task CreateEntry(string[] strings)
{
    var hasNotPatronymic = DateTime.TryParse(strings[3], out var birthday);
    var customer = new Customer
    {
        FirstName = strings[1],
        LastName = strings[2]
    };

    if (!hasNotPatronymic)
    {
        customer.Patronymic = strings[3];
        _ = DateTime.TryParse(strings[4], out birthday);
        customer.GenderName = strings[5];
    }
    else
    {
        customer.GenderName = strings[4];
    }

    customer.Birthday = birthday;
    await DbService.CreateLine(customer);
}

async Task ShowTable()
{
    var customers = await DbService.ShowTable();
    if (customers == null)
    {
        Console.WriteLine("Произошла ошибка");
        return;
    }

    foreach (var item in customers)
    {
        Console.Write(
            $"{item.FirstName}\t{item.LastName}\t{item.Patronymic}\t{item.Birthday.Date:dd.MM.yyyy}\t{item.GenderName}\t{item.Age}\n");
    }
}

async Task ShowEntriesWithFilter(Stopwatch stopwatch)
{
    stopwatch.Restart();
    var list = await DbService.ShowEntriesManWithFirstF();
    stopwatch.Stop();

    if (list == null)
    {
        Console.WriteLine("Произошла ошибка");
        return;
    }


    foreach (var item in list)
    {
        Console.WriteLine(
            $"{item.FirstName}\t{item.LastName}\t{item.Patronymic}\t{item.Birthday.Date:dd.MM.yyyy}\t{item.GenderName}\n");
    }

    Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} миллисекунды");
}
