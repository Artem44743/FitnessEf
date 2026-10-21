using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

await using var db = new FitnessContext();

Console.WriteLine("=== 8.1. Клиенты без Email (NULL) ===");
var clientsWithoutEmail = await db.Clients
    .AsNoTracking()
    .Where(c => c.Email == null)
    .OrderBy(c => c.ClientId)
    .ToListAsync();

foreach (var c in clientsWithoutEmail)
{
    Console.WriteLine($"ID {c.ClientId}: {c.FirstName} {c.LastName} (Email отсутствует)");
}

Console.WriteLine("\n=== 8.1. Клиенты с Email (безопасный вызов Trim) ===");
var clientsWithEmail = await db.Clients
    .AsNoTracking()
    .Where(c => c.Email != null)
    .OrderBy(c => c.ClientId)
    .ToListAsync();

foreach (var c in clientsWithEmail)
{
    string safeEmail = c.Email?.Trim() ?? "неизвестно";
    Console.WriteLine($"ID {c.ClientId}: {c.FirstName} {c.LastName} — {safeEmail}");
}