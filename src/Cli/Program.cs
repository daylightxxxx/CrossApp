using Core.Domain;
using Core.Dto;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("=== Сценарій 1: успіх ===");
Order order = Order.Create("O-001", "C-001");
order.AddLine("P-001", "Ноутбук Lenovo IdeaPad", 24999.99m, 1);
order.AddLine("P-002", "Миша бездротова Logitech", 499.50m, 2);
Console.WriteLine(order);
order.Confirm();
Console.WriteLine(order);
Console.WriteLine();

Console.WriteLine("=== Сценарій 2: порушення інваріантів ===");
Order draft = Order.Create("O-002", "C-001");

TryDo("рядок до підтвердженого замовлення", () => order.AddLine("P-003", "Клавіатура", 1899m, 1));
TryDo("повторне підтвердження", () => order.Confirm());
TryDo("підтвердження порожнього замовлення", () => draft.Confirm());
TryDo("нульова кількість у рядку", () => draft.AddLine("P-003", "Клавіатура", 1899m, 0));
TryDo("від'ємна ціна", () => draft.AddLine("P-003", "Клавіатура", -5m, 1));
TryDo("порожній клієнт", () => Order.Create("O-003", " "));
Console.WriteLine();

Console.WriteLine("=== Стан після відмов (має бути без змін) ===");
Console.WriteLine(order);
Console.WriteLine(draft);
Console.WriteLine();

Console.WriteLine("=== Мапінг ToDto / FromDto ===");
OrderDto dto = order.ToDto();
Order restored = Order.FromDto(dto);
Console.WriteLine(restored);
OrderDto broken = dto with { Lines = [new OrderLineDto("P-001", "Товар", 10m, -1)] };
TryDo("FromDto з пошкодженими даними", () => Order.FromDto(broken));

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"  {title}: виняток НЕ спрацював — інваріант відсутній!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  {title}: {ex.GetType().Name} — {ex.Message}");
    }
}