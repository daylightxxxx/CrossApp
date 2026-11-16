using Core.Dto;

namespace Core.Domain;

public sealed class Order
{
    private readonly List<OrderLine> _lines = [];

    public string Id { get; }
    public string CustomerId { get; }
    public bool IsConfirmed { get; private set; }
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    public decimal Total => _lines.Sum(l => l.Total);

    private Order(string id, string customerId)
    {
        Id = id;
        CustomerId = customerId;
    }

    // Єдиний спосіб створити замовлення: усі перевірки тут.
    public static Order Create(string id, string customerId)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор замовлення обов'язковий", nameof(id));
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Ідентифікатор клієнта обов'язковий", nameof(customerId));

        return new Order(id.Trim(), customerId.Trim());
    }

    public void AddLine(string productId, string name, decimal price, int quantity)
    {
        if (IsConfirmed)
            throw new InvalidOperationException(
                $"Замовлення {Id} вже підтверджене, рядки додавати не можна");
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Ідентифікатор товару обов'язковий", nameof(productId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Назва товару не може бути порожньою", nameof(name));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), quantity,
                "Кількість у рядку має бути більшою за нуль");
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), price,
                "Ціна не може бути від'ємною");

        _lines.Add(new OrderLine(productId.Trim(), name.Trim(), price, quantity));
    }

    public void Confirm()
    {
        if (IsConfirmed)
            throw new InvalidOperationException($"Замовлення {Id} вже підтверджене");
        if (_lines.Count == 0)
            throw new InvalidOperationException(
                $"Не можна підтвердити порожнє замовлення {Id}: немає жодного рядка");

        IsConfirmed = true;
    }

    // Мапінг у формат тижня 3 і назад (знадобиться сховищу тижня 5).
    public OrderDto ToDto() => new(
        Id,
        CustomerId,
        IsConfirmed,
        _lines.Select(l => new OrderLineDto(l.ProductId, l.Name, l.Price, l.Quantity)).ToList());

    // FromDto проходить ті самі перевірки: через Create і AddLine, не в обхід.
    public static Order FromDto(OrderDto dto)
    {
        Order order = Create(dto.Id, dto.CustomerId);
        foreach (OrderLineDto line in dto.Lines)
            order.AddLine(line.ProductId, line.Name, line.Price, line.Quantity);
        if (dto.IsConfirmed)
            order.Confirm();
        return order;
    }

    public override string ToString() =>
        $"Замовлення {Id} (клієнт {CustomerId}): рядків {_lines.Count}, сума {Total:F2}, " +
        (IsConfirmed ? "підтверджене" : "чернетка");
}