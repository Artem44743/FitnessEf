using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

await using var db = new FitnessContext();

Console.WriteLine("=== 7.1. Изменение телефона клиента с ID = 1 ===");
int idToUpdate = 1;
var clientToUpdate = await db.Clients.FirstOrDefaultAsync(c => c.ClientId == idToUpdate);

if (clientToUpdate is null)
{
    Console.WriteLine("Клиент не найден.");
}
else
{
    Console.WriteLine($"Старый телефон: {clientToUpdate.Phone ?? "не указан"}");
    clientToUpdate.Phone = "79990001122";
    await db.SaveChangesAsync();
    Console.WriteLine("Телефон изменён на 79990001122.");
}

Console.WriteLine("\n=== 7.4. Добавление нового и изменение существующего ===");
db.Clients.Add(new Client
{
    FirstName = "Анна",
    LastName = "Смирнова",
    RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
    Status = "active"
});

var existing = await db.Clients.FindAsync(1);
if (existing is not null)
{
    existing.Status = "inactive";
}

int saved = await db.SaveChangesAsync();
Console.WriteLine($"Сохранено изменений (добавление + обновление): {saved}");
Console.WriteLine("Клиент с ID=1 переведён в статус inactive.");

Console.WriteLine("\n=== 7.2. Удаление клиента ===");
Console.Write("Введите ID клиента для удаления (например, 3): ");
if (int.TryParse(Console.ReadLine(), out int idToDelete))
{
    var clientToDelete = await db.Clients.FindAsync(idToDelete);

    if (clientToDelete is null)
    {
        Console.WriteLine("Клиент не найден.");
    }
    else
    {
        db.Clients.Remove(clientToDelete);
        await db.SaveChangesAsync();
        Console.WriteLine($"Клиент с ID {idToDelete} удалён.");
    }
}
else
{
    Console.WriteLine("Некорректный ID.");
}