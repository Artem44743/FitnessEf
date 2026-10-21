using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

while (true)
{
    Console.WriteLine("\n--- МЕНЮ ---");
    Console.WriteLine("1. Список клиентов");
    Console.WriteLine("2. Найти клиента по ID");
    Console.WriteLine("3. Добавить клиента");
    Console.WriteLine("4. Изменить телефон/статус");
    Console.WriteLine("5. Удалить клиента");
    Console.WriteLine("6. Статистика");
    Console.WriteLine("0. Выход");
    Console.Write("Выбор: ");

    string choice = Console.ReadLine() ?? string.Empty;
    if (choice == "0") break;

    try
    {
        switch (choice)
        {
            case "1": await ListClientsAsync(); break;
            case "2": await FindClientAsync(); break;
            case "3": await AddClientAsync(); break;
            case "4": await UpdateClientAsync(); break;
            case "5": await DeleteClientAsync(); break;
            case "6": await ShowStatisticsAsync(); break;
            default: Console.WriteLine("Неизвестный пункт меню."); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

// --- РЕАЛИЗАЦИЯ МЕТОДОВ ---

async Task ListClientsAsync()
{
    await using var db = new FitnessContext();
    var clients = await db.Clients.AsNoTracking()
        .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
        .ToListAsync();

    Console.WriteLine("\nСписок клиентов:");
    foreach (var c in clients)
        Console.WriteLine($"{c.ClientId}: {c.LastName} {c.FirstName} | Тел: {c.Phone ?? "нет"} | Статус: {c.Status}");
}

async Task FindClientAsync()
{
    Console.Write("Введите ID клиента: ");
    if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
    {
        Console.WriteLine("Неверный ID.");
        return;
    }

    await using var db = new FitnessContext();
    var client = await db.Clients.FindAsync(id);
    if (client == null)
        Console.WriteLine("Клиент не найден.");
    else
        Console.WriteLine($"{client.ClientId}: {client.LastName} {client.FirstName} | Тел: {client.Phone ?? "нет"} | Статус: {client.Status}");
}

async Task AddClientAsync()
{
    Console.Write("Имя: ");
    string firstName = Console.ReadLine()?.Trim() ?? "";
    Console.Write("Фамилия: ");
    string lastName = Console.ReadLine()?.Trim() ?? "";

    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
    {
        Console.WriteLine("Имя и фамилия обязательны.");
        return;
    }

    await using var db = new FitnessContext();
    var client = new Client
    {
        FirstName = firstName,
        LastName = lastName,
        RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
        Status = "active"
    };

    db.Clients.Add(client);
    await db.SaveChangesAsync();
    Console.WriteLine($"Клиент добавлен. ID: {client.ClientId}");
}

async Task UpdateClientAsync()
{
    Console.Write("ID клиента для изменения: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    await using var db = new FitnessContext();
    var client = await db.Clients.FindAsync(id);
    if (client == null)
    {
        Console.WriteLine("Клиент не найден.");
        return;
    }

    Console.Write($"Новый телефон (текущий: {client.Phone ?? "нет"}): ");
    string phone = Console.ReadLine()?.Trim() ?? "";
    if (!string.IsNullOrEmpty(phone)) client.Phone = phone;

    Console.Write($"Новый статус (текущий: {client.Status}): ");
    string status = Console.ReadLine()?.Trim() ?? "";
    if (!string.IsNullOrEmpty(status)) client.Status = status;

    await db.SaveChangesAsync();
    Console.WriteLine("Данные обновлены.");
}

async Task DeleteClientAsync()
{
    Console.Write("ID клиента для удаления: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    await using var db = new FitnessContext();
    var client = await db.Clients.FindAsync(id);
    if (client == null)
    {
        Console.WriteLine("Клиент не найден.");
        return;
    }

    try
    {
        db.Clients.Remove(client);
        await db.SaveChangesAsync();
        Console.WriteLine("Клиент удален.");
    }
    catch (DbUpdateException)
    {
        Console.WriteLine("Нельзя удалить клиента, так как у него есть связанные посещения (ограничение внешнего ключа).");
    }
}

async Task ShowStatisticsAsync()
{
    await using var db = new FitnessContext();
    int total = await db.Clients.CountAsync();
    int active = await db.Clients.CountAsync(c => c.Status == "active");

    Console.WriteLine($"\nВсего клиентов: {total}");
    Console.WriteLine($"Активных клиентов: {active}");
}