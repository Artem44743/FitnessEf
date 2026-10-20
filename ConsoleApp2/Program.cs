using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

var clients = await db.Clients
    .AsNoTracking()
    .OrderBy(c => c.LastName)
    .ThenBy(c => c.FirstName)
    .Select(c => new
    {
        c.ClientId,
        FullName = c.FirstName + " " + c.LastName,
        c.Phone
    })
    .ToListAsync();

foreach (var c in clients)
{
    Console.WriteLine($"{c.ClientId}: {c.FullName}; тел.: {c.Phone ?? "не указан"}");
}