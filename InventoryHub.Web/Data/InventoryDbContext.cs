using InventoryHub.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Web.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Product>().HasIndex(p => p.Name).HasDatabaseName("IX_Products_Name");
        mb.Entity<Product>().HasIndex(p => new { p.CategoryId, p.IsActive });
        mb.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        mb.Entity<Product>()
          .HasOne(p => p.Category)
          .WithMany(c => c.Products)
          .HasForeignKey(p => p.CategoryId)
          .OnDelete(DeleteBehavior.Restrict);
    }
}