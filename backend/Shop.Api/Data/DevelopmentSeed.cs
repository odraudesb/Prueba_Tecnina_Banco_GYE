using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Models;

namespace Shop.Api.Data;

internal static class DevelopmentSeed
{
    public static async Task RunAsync(ShopDbContext db, string password)
    {
        const string email = "demo@shop.local";
        if (!await db.Users.AnyAsync(user => user.Email == email))
        {
            var user = new User { Email = email, PasswordHash = string.Empty };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, password);
            db.Users.Add(user);
        }

        Product[] products =
        [
            new() { Code = "TEC-001", Name = "Teclado mecánico", Category = "Tecnología", Price = 70m, Stock = 10 },
            new() { Code = "TEC-002", Name = "Mouse inalámbrico", Category = "Tecnología", Price = 30m, Stock = 15 },
            new() { Code = "HOG-001", Name = "Lámpara de escritorio", Category = "Hogar", Price = 35.50m, Stock = 8 }
        ];

        var codes = products.Select(product => product.Code).ToArray();
        var existingCodes = await db.Products.Where(product => codes.Contains(product.Code))
            .Select(product => product.Code).ToListAsync();
        db.Products.AddRange(products.Where(product => !existingCodes.Contains(product.Code)));
        await db.SaveChangesAsync();
    }
}
