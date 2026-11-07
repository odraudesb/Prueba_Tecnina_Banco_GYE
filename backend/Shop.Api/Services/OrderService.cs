using Microsoft.EntityFrameworkCore;
using Shop.Api.Contracts;
using Shop.Api.Data;
using Shop.Api.Models;

namespace Shop.Api.Services;

public sealed class OrderService(ShopDbContext db, CartService carts)
{
    public async Task<OrderResponse> CheckoutAsync(int userId, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.LockCartAsync(userId, cancellationToken);
        var cart = await carts.GetAsync(userId, cancellationToken);

        if (cart.Items.Count == 0)
            throw new ShopRequestException(StatusCodes.Status409Conflict, "El carrito está vacío.");

        foreach (var line in cart.Items.OrderBy(line => line.ProductId))
        {
            var updated = await db.Products
                .Where(product => product.Id == line.ProductId && product.Stock >= line.Quantity)
                .ExecuteUpdateAsync(update => update.SetProperty(
                    product => product.Stock, product => product.Stock - line.Quantity), cancellationToken);

            if (updated == 0)
                throw new ShopRequestException(StatusCodes.Status409Conflict, $"Stock insuficiente para {line.Name}.");
        }

        var order = new Order
        {
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow,
            Subtotal = cart.Subtotal,
            Discount = cart.Discount,
            Total = cart.Total,
            Items = cart.Items.Select(line => new OrderItem
            {
                ProductId = line.ProductId,
                ProductName = line.Name,
                UnitPrice = line.UnitPrice,
                Quantity = line.Quantity
            }).ToList()
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync(cancellationToken);
        await db.CartItems.Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ToResponse(order);
    }

    public Task<List<OrderSummary>> ListAsync(int userId, CancellationToken cancellationToken) =>
        db.Orders.AsNoTracking().Where(order => order.UserId == userId)
            .OrderByDescending(order => order.CreatedAt).ThenByDescending(order => order.Id)
            .Select(order => new OrderSummary(order.Id, order.CreatedAt, order.Subtotal, order.Discount, order.Total))
            .ToListAsync(cancellationToken);

    public async Task<OrderResponse?> FindAsync(int userId, int id, CancellationToken cancellationToken)
    {
        var order = await db.Orders.AsNoTracking().Include(order => order.Items)
            .SingleOrDefaultAsync(order => order.UserId == userId && order.Id == id, cancellationToken);
        return order is null ? null : ToResponse(order);
    }

    private static OrderResponse ToResponse(Order order) => new(
        order.Id, order.CreatedAt, order.Subtotal, order.Discount, order.Total,
        order.Items.OrderBy(item => item.ProductId)
            .Select(item => new OrderLineResponse(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity))
            .ToArray());
}
