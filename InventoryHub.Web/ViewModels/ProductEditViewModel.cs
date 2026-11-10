using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Web.ViewModels;

public class ProductEditViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string Sku { get; set; } = string.Empty;

    [Range(0.01, 100000)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}