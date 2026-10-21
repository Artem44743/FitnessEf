using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

await using var db = new FitnessContext();

Console.WriteLine("=== 6.2. Проверка пользовательского ввода ===");
Console.Write("Введите имя: ");
string firstName = Console.ReadLine()?.Trim() ?? string.Empty;

Console.Write("Введите фамилию: ");
string lastName = Console.ReadLine()?.Trim() ?? string.Empty;

if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
{
    Console.WriteLine("Ошибка: Имя и фамилия обязательны.");
    return;
}

var client = new Client
{
    FirstName = firstName,
    LastName = lastName,
    RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
    Status = "active"
};

Console.WriteLine("\n=== 6.1 и 6.3. Сохранение и обработка ошибок ===");

try
{
    db.Clients.Add(client);

    int saved = await db.SaveChangesAsync();

    Console.WriteLine($"Сохранено изменений: {saved}");
    Console.WriteLine($"Клиент успешно добавлен. ID нового клиента: {client.ClientId}");
}
catch (DbUpdateException ex)
{
    Console.WriteLine("Не удалось сохранить клиента. Проверьте данные и ограничения БД.");
    Console.WriteLine("Детали ошибки: " + ex.GetBaseException().Message);
}