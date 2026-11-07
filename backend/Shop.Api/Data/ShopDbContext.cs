using Microsoft.EntityFrameworkCore;
using Shop.Api.Models;

namespace Shop.Api.Data;

public sealed class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public async Task LockCartAsync(int userId, CancellationToken cancellationToken)
    {
        await Database.SqlQuery<int>(
                $"SELECT [Id] AS [Value] FROM [dbo].[Users] WITH (UPDLOCK, HOLDLOCK) WHERE [Id] = {userId}")
            .SingleAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var users = modelBuilder.Entity<User>();
        users.Property(user => user.Email).HasMaxLength(254);
        users.Property(user => user.PasswordHash).HasMaxLength(256);
        users.HasIndex(user => user.Email).IsUnique();

        var products = modelBuilder.Entity<Product>();
        products.Property(product => product.Code).HasMaxLength(32);
        products.Property(product => product.Name).HasMaxLength(120);
        products.Property(product => product.Category).HasMaxLength(80);
        products.Property(product => product.Price).HasPrecision(12, 2);
        products.HasIndex(product => product.Code).IsUnique();
        products.ToTable(table =>
        {
            table.HasCheckConstraint("CK_Products_Stock", "[Stock] >= 0");
            table.HasCheckConstraint("CK_Products_Price", "[Price] >= 0");
        });

        var cart = modelBuilder.Entity<CartItem>();
        cart.HasKey(item => new { item.UserId, item.ProductId });
        cart.HasOne<User>().WithMany().HasForeignKey(item => item.UserId);
        cart.HasOne<Product>().WithMany().HasForeignKey(item => item.ProductId).OnDelete(DeleteBehavior.Restrict);
        cart.ToTable(table => table.HasCheckConstraint("CK_CartItems_Quantity", "[Quantity] > 0"));

        var orders = modelBuilder.Entity<Order>();
        orders.HasOne<User>().WithMany().HasForeignKey(order => order.UserId).OnDelete(DeleteBehavior.Restrict);
        orders.HasIndex(order => new { order.UserId, order.CreatedAt });
        orders.Property(order => order.Subtotal).HasPrecision(18, 2);
        orders.Property(order => order.Discount).HasPrecision(18, 2);
        orders.Property(order => order.Total).HasPrecision(18, 2);
        orders.ToTable(table => table.HasCheckConstraint(
            "CK_Orders_Totals", "[Subtotal] >= 0 AND [Discount] >= 0 AND [Total] >= 0 AND [Total] = [Subtotal] - [Discount]"));

        var orderItems = modelBuilder.Entity<OrderItem>();
        orderItems.HasKey(item => new { item.OrderId, item.ProductId });
        orderItems.HasOne<Order>().WithMany(order => order.Items).HasForeignKey(item => item.OrderId);
        orderItems.HasOne<Product>().WithMany().HasForeignKey(item => item.ProductId).OnDelete(DeleteBehavior.Restrict);
        orderItems.Property(item => item.ProductName).HasMaxLength(120);
        orderItems.Property(item => item.UnitPrice).HasPrecision(12, 2);
        orderItems.ToTable(table => table.HasCheckConstraint("CK_OrderItems_Values", "[Quantity] > 0 AND [UnitPrice] >= 0"));
    }
}
