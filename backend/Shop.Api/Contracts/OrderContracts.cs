namespace Shop.Api.Contracts;

public sealed record OrderSummary(int Id, DateTimeOffset CreatedAt, decimal Subtotal, decimal Discount, decimal Total);

public sealed record OrderLineResponse(int ProductId, string ProductName, decimal UnitPrice, int Quantity)
{
    public decimal Subtotal => UnitPrice * Quantity;
}

public sealed record OrderResponse(
    int Id,
    DateTimeOffset CreatedAt,
    decimal Subtotal,
    decimal Discount,
    decimal Total,
    IReadOnlyList<OrderLineResponse> Items);
