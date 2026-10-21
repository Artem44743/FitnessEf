using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

try
{
    await using var db = new FitnessContext();

    Console.WriteLine("=== 11.2. Асинхронное выполнение ===");
    var count = await db.Clients.CountAsync();
    var first = await db.Clients
        .AsNoTracking()
        .OrderBy(c => c.ClientId)
        .FirstOrDefaultAsync();

    Console.WriteLine($"Всего клиентов (CountAsync): {count}");
    if (first != null)
        Console.WriteLine($"Первый клиент (FirstOrDefaultAsync): {first.FirstName} {first.LastName}");

    Console.WriteLine("\n=== 11.3. Tracking vs NoTracking ===");

    var noTrackingList = await db.Clients.AsNoTracking().ToListAsync();
    Console.WriteLine($"Прочитано без отслеживания: {noTrackingList.Count} записей.");

    int idToUpdate = 1;
    var clientToUpdate = await db.Clients.FirstOrDefaultAsync(c => c.ClientId == idToUpdate);
    if (clientToUpdate is not null)
    {
        Console.WriteLine($"Старый статус клиента ID {idToUpdate}: {clientToUpdate.Status}");
        clientToUpdate.Status = "inactive";
        await db.SaveChangesAsync();
        Console.WriteLine("Статус изменен на 'inactive' (сохранено).");
    }

    Console.WriteLine("\n=== 11.4. Диагностика SQL ===");
    var query = db.Clients
        .Where(c => c.Status == "active")
        .OrderBy(c => c.LastName);

    Console.WriteLine("Сгенерированный SQL-запрос для активных клиентов:");
    Console.WriteLine(query.ToQueryString());
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Ошибка конфигурации: {ex.Message}");
}
catch (DbUpdateException ex)
{
    Console.WriteLine("Ошибка сохранения данных.");
    Console.WriteLine(ex.GetBaseException().Message);
}
catch (Exception ex)
{
    Console.WriteLine("Произошла непредвиденная ошибка.");
    Console.WriteLine(ex.Message);
}