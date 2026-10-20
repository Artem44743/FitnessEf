using FitnessEf;
using Microsoft.EntityFrameworkCore;


Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

await using var db = new FitnessContext();

Console.WriteLine("=== 5.1. Фильтрация (активные клиенты) ===");
var activeClients = await db.Clients
    .AsNoTracking()
    .Where(c => c.Status == "active")
    .OrderBy(c => c.LastName)
    .ToListAsync();

foreach (var c in activeClients)
    Console.WriteLine($"- {c.FirstName} {c.LastName}");

Console.WriteLine("\n=== 5.2. Поиск по фамилии ===");
Console.Write("Фрагмент фамилии: ");
string term = Console.ReadLine()?.Trim() ?? string.Empty;
if (term.Length == 0)
    Console.WriteLine("Строка поиска не задана.");
else
{
    var found = await db.Clients.AsNoTracking()
        .Where(c => c.LastName.Contains(term))
        .OrderBy(c => c.LastName)
        .ToListAsync();
    foreach (var c in found)
        Console.WriteLine($"{c.ClientId}: {c.FirstName} {c.LastName}");
}

Console.WriteLine("\n=== 5.3. Поиск по ID ===");
Console.Write("ID клиента (например, 1): ");
if (int.TryParse(Console.ReadLine(), out int id) && id > 0)
{
    var client = await db.Clients.FindAsync(id);
    if (client is null)
        Console.WriteLine("Клиент не найден.");
    else
        Console.WriteLine($"{client.ClientId}: {client.FirstName} {client.LastName}");
}
else
{
    Console.WriteLine("ID должен быть положительным целым числом.");
}

Console.WriteLine("\n=== 5.4. Агрегаты (количество) ===");
int total = await db.Clients.CountAsync();
bool hasInactive = await db.Clients.AnyAsync(c => c.Status == "inactive");
int activeCount = await db.Clients.CountAsync(c => c.Status == "active");

Console.WriteLine($"Всего клиентов: {total}");
Console.WriteLine($"Активных клиентов: {activeCount}");
Console.WriteLine(hasInactive ? "Есть неактивные клиенты." : "Неактивные клиенты отсутствуют.");

Console.WriteLine("\n=== 5.5. Постраничная загрузка (Стр 1) ===");
int page = 1;
int pageSize = 5;
var pageOfClients = await db.Clients.AsNoTracking()
    .OrderBy(c => c.ClientId)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

foreach (var c in pageOfClients)
    Console.WriteLine($"{c.ClientId}: {c.FirstName}");