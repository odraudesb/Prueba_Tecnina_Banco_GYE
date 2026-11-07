using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Contracts;
using Shop.Api.Data;
using Shop.Api.Models;

namespace Shop.Api.Services;

public sealed class ProductService(ShopDbContext db)
{
    private static readonly Expression<Func<Product, ProductResponse>> ToResponse = product =>
        new ProductResponse(product.Id, product.Code, product.Name, product.Category, product.Price, product.Stock);

    public Task<List<ProductResponse>> ListAsync(string? search, string? category, CancellationToken cancellationToken)
    {
        var query = db.Products.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(product =>
                product.Name.Contains(term) || product.Code.Contains(term) || product.Category.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var value = category.Trim();
            query = query.Where(product => product.Category == value);
        }

        return query.OrderBy(product => product.Name).ThenBy(product => product.Id)
            .Select(ToResponse).ToListAsync(cancellationToken);
    }

    public Task<ProductResponse?> FindAsync(int id, CancellationToken cancellationToken) =>
        db.Products.AsNoTracking().Where(product => product.Id == id)
            .Select(ToResponse).SingleOrDefaultAsync(cancellationToken);
}
