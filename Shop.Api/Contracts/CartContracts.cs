using System.ComponentModel.DataAnnotations;

namespace Shop.Api.Contracts;

public sealed record AddCartItemRequest(
    [Range(1, int.MaxValue)] int ProductId,
    [Range(1, int.MaxValue)] int Quantity);

public sealed record UpdateCartItemRequest([Range(1, int.MaxValue)] int Quantity);

public sealed record CartLine(int ProductId, string Code, string Name, decimal UnitPrice, int Quantity, int Stock)
{
    public decimal Subtotal => UnitPrice * Quantity;
}

public sealed record CartResponse(IReadOnlyList<CartLine> Items, decimal Subtotal, decimal Discount, decimal Total);
