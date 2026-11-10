using InventoryHub.Web.Dtos;

namespace InventoryHub.Web.ViewModels;

public class ProductListViewModel
{
    public PagedResponse<ProductResponse> Data { get; set; } = new(Array.Empty<ProductResponse>(), 1, 10, 0, 0);
    public List<CategoryResponse> Categories { get; set; } = new();
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
}