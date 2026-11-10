using InventoryHub.Web.Models;

namespace InventoryHub.Web.Data;

public static class DbSeeder
{
    public static void Seed(InventoryDbContext db)
    {
        if (db.Categories.Any()) return;

        var electronics = new Category { Name = "Electronics" };
        var office = new Category { Name = "Office" };
        db.Categories.AddRange(electronics, office);
        db.SaveChanges();

        db.Products.AddRange(
            new Product { Name = "USB-C Hub", Sku = "EL-001", Price = 49.99m, Stock = 120, CategoryId = electronics.Id },
            new Product { Name = "Wireless Mouse", Sku = "EL-002", Price = 19.50m, Stock = 300, CategoryId = electronics.Id },
            new Product { Name = "Notebook A5", Sku = "OF-001", Price = 7.99m, Stock = 500, CategoryId = office.Id },
            new Product { Name = "Desk Lamp", Sku = "OF-002", Price = 29.90m, Stock = 80, CategoryId = office.Id }
        );
        db.SaveChanges();
    }
}