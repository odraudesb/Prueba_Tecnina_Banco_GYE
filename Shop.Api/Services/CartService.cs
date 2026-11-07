using Microsoft.EntityFrameworkCore;
using Shop.Api.Contracts;
using Shop.Api.Data;
using Shop.Api.Models;

namespace Shop.Api.Services;

public sealed class CartService(ShopDbContext db)
{
    public async Task<CartResponse> GetAsync(int userId, CancellationToken cancellationToken)
    {
        var lines = await (
            from item in db.CartItems
            join product in db.Products on item.ProductId equals product.Id
            where item.UserId == userId
            orderby product.Id
            select new CartLine(product.Id, product.Code, product.Name, product.Price, item.Quantity, product.Stock)
        ).ToListAsync(cancellationToken);

        var subtotal = lines.Sum(line => line.Subtotal);
        var discount = subtotal > 100m
            ? Math.Round(subtotal * 0.10m, 2, MidpointRounding.AwayFromZero)
            : 0m;
        return new CartResponse(lines, subtotal, discount, subtotal - discount);
    }

    public Task<CartResponse> AddAsync(int userId, int productId, int quantity, CancellationToken cancellationToken) =>
        SetQuantityAsync(userId, productId, quantity, add: true, cancellationToken);

    public Task<CartResponse> UpdateAsync(int userId, int productId, int quantity, CancellationToken cancellationToken) =>
        SetQuantityAsync(userId, productId, quantity, add: false, cancellationToken);

    public async Task<CartResponse> RemoveAsync(int userId, int productId, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.LockCartAsync(userId, cancellationToken);
        var deleted = await db.CartItems
            .Where(item => item.UserId == userId && item.ProductId == productId)
            .ExecuteDeleteAsync(cancellationToken);
        if (deleted == 0)
            throw new ShopRequestException(StatusCodes.Status404NotFound, "El producto no está en el carrito.");

        var cart = await GetAsync(userId, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return cart;
    }

    public async Task<CartResponse> ClearAsync(int userId, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.LockCartAsync(userId, cancellationToken);
        await db.CartItems.Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new CartResponse([], 0, 0, 0);
    }

    private async Task<CartResponse> SetQuantityAsync(
        int userId, int productId, int quantity, bool add, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.LockCartAsync(userId, cancellationToken);
        var item = await db.CartItems.FindAsync([userId, productId], cancellationToken);
        if (!add && item is null)
            throw new ShopRequestException(StatusCodes.Status404NotFound, "El producto no está en el carrito.");

        var product = await db.Products.AsNoTracking()
            .SingleOrDefaultAsync(product => product.Id == productId, cancellationToken);
        if (product is null)
            throw new ShopRequestException(StatusCodes.Status404NotFound, "Producto no encontrado.");

        var nextQuantity = add ? (long)(item?.Quantity ?? 0) + quantity : quantity;
        if (nextQuantity > product.Stock)
            throw new ShopRequestException(StatusCodes.Status409Conflict, "Stock insuficiente.");

        if (item is null)
            db.CartItems.Add(new CartItem { UserId = userId, ProductId = productId, Quantity = (int)nextQuantity });
        else
            item.Quantity = (int)nextQuantity;

        await db.SaveChangesAsync(cancellationToken);
        var cart = await GetAsync(userId, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return cart;
    }
}
