using System.Globalization;
using Core.Dto;

namespace Core.Import;

public abstract record ImportedEntity;
public sealed record ImportedProduct(ProductDto Value) : ImportedEntity;
public sealed record ImportedCustomer(CustomerDto Value) : ImportedEntity;

public static class MixedCsvImporter
{
    private const char Separator = ';';

    public static ImportResult<ImportedEntity> Load(string path)
    {
        var items = new List<ImportedEntity>();
        var errors = new List<string>();

        string[] lines = File.ReadAllLines(path, System.Text.Encoding.UTF8);

        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

            switch (parts)
            {
                case ["P", var id, "", _]:
                    errors.Add($"рядок {number}: назва товару порожня");
                    break;

                case ["P", var id, var name, var price] when !decimal.TryParse(price, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal p) || p < 0:
                    errors.Add($"рядок {number}: ціна '{price}' некоректна");
                    break;

                case ["P", var id, var name, var price]:
                    items.Add(new ImportedProduct(new ProductDto(id, name,
                        decimal.Parse(price, NumberStyles.Number, CultureInfo.InvariantCulture))));
                    break;

                case ["C", var id, var name, var phone]:
                    items.Add(new ImportedCustomer(new CustomerDto(id, name,
                        string.IsNullOrWhiteSpace(phone) ? null : phone)));
                    break;

                case [var prefix, ..]:
                    errors.Add($"рядок {number}: невідомий префікс '{prefix}'");
                    break;

                default:
                    errors.Add($"рядок {number}: некоректний формат");
                    break;
            }
        }

        return new ImportResult<ImportedEntity>(items, errors);
    }
}