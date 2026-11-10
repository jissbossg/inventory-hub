using InventoryHub.Web.Dtos;

namespace InventoryHub.Web.Services;

public interface IProductService
{
    Task<PagedResponse<ProductResponse>> GetPagedAsync(int page, int pageSize, string? search, int? categoryId, CancellationToken ct);
    Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync(CancellationToken ct);
    Task<ProductResponse> UpdateStockAsync(int id, int delta, string? reason, CancellationToken ct);
}