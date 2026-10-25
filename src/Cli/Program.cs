using Core.Dto;
using Core.Import;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

ImportResult<ProductDto> result = Path.GetExtension(path).ToLowerInvariant() switch
{
    ".csv" => ProductCsvImporter.Load(path),
    ".json" => ProductJsonImporter.Load(path),
    var ext => throw new NotSupportedException($"Непідтримуване розширення файлу: {ext}")
};

Console.WriteLine($"Завантажено записів: {result.Items.Count}");
foreach (ProductDto p in result.Items.Take(5))
    Console.WriteLine($"  {p.Id,-6} {p.Name,-30} {p.Price,10:F2}");

if (result.Errors.Count > 0)
{
    Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");
    foreach (string e in result.Errors)
        Console.WriteLine($"  ! {e}");
}

if (path.Contains("mixed"))
{
    var mixed = MixedCsvImporter.Load(path);
    Console.WriteLine($"[mixed] Завантажено сутностей: {mixed.Items.Count}");
    foreach (var e in mixed.Items)
        Console.WriteLine(e switch
        {
            ImportedProduct p => $"  товар:   {p.Value.Id} {p.Value.Name} {p.Value.Price:F2}",
            ImportedCustomer c => $"  клієнт:  {c.Value.Id} {c.Value.Name} {c.Value.Phone ?? "—"}",
            _ => "  ?"
        });
    if (mixed.Errors.Count > 0)
    {
        Console.WriteLine($"[mixed] Помилок: {mixed.Errors.Count}");
        foreach (var err in mixed.Errors) Console.WriteLine($"  ! {err}");
    }
}

int total = result.Items.Count + result.Errors.Count;
double errorRate = total > 0 ? (double)result.Errors.Count / total * 100 : 0;
Console.WriteLine($"Статистика: усього {total}, прийнято {result.Items.Count}, пропущено {result.Errors.Count}, помилок {errorRate:F1}%");

return 0;

