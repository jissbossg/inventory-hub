using InventoryHub.Web.Data;
using InventoryHub.Web.Dtos;
using InventoryHub.Web.Hubs;
using InventoryHub.Web.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Web.Services;

public class ProductService : IProductService
{
    private readonly InventoryDbContext _db;
    private readonly ICacheService _cache;
    private readonly IConfiguration _cfg;
    private readonly IHubContext<InventoryHubHub> _hub;

    public ProductService(InventoryDbContext db, ICacheService cache,
        IConfiguration cfg, IHubContext<InventoryHubHub> hub)
    {
        _db = db; _cache = cache; _cfg = cfg; _hub = hub;
    }

    public async Task<PagedResponse<ProductResponse>> GetPagedAsync(int page, int pageSize, string? search, int? categoryId, CancellationToken ct)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var key = $"products:p{page}:s{pageSize}:q{search ?? "-"}:c{categoryId?.ToString() ?? "-"}";
        if (_cache.TryGet<PagedResponse<ProductResponse>>(key, out var cached) && cached is not null)
            return cached;

        var q = _db.Products.AsNoTracking().Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(p => EF.Functions.Like(p.Name, $"%{search}%") || EF.Functions.Like(p.Sku, $"%{search}%"));

        if (categoryId is > 0) q = q.Where(p => p.CategoryId == categoryId);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductResponse(
                p.Id, p.Name, p.Sku, p.Price, p.Stock,
                p.CategoryId, p.Category!.Name, p.IsActive, p.UpdatedUtc))
            .ToListAsync(ct);

        var result = new PagedResponse<ProductResponse>(
            items, page, pageSize, total,
            pageSize == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize));

        _cache.Set(key, result, TimeSpan.FromSeconds(_cfg.GetValue("Cache:ProductsSeconds", 20)));
        return result;
    }

    public async Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var key = $"product:{id}";
        if (_cache.TryGet<ProductResponse>(key, out var cached) && cached is not null) return cached;

        var dto = await _db.Products.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductResponse(
                p.Id, p.Name, p.Sku, p.Price, p.Stock,
                p.CategoryId, p.Category!.Name, p.IsActive, p.UpdatedUtc))
            .FirstOrDefaultAsync(ct);

        if (dto is not null) _cache.Set(key, dto, TimeSpan.FromSeconds(20));
        return dto;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync(CancellationToken ct)
    {
        const string key = "categories:all";
        if (_cache.TryGet<IReadOnlyList<CategoryResponse>>(key, out var cached) && cached is not null) return cached;

        var list = await _db.Categories.AsNoTracking()
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Products.Count))
            .ToListAsync(ct);

        _cache.Set(key, list, TimeSpan.FromSeconds(180));
        return list;
    }

    public async Task<ProductResponse> UpdateStockAsync(int id, int delta, string? reason, CancellationToken ct)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new KeyNotFoundException($"Product {id} not found");

        var newStock = product.Stock + delta;
        if (newStock < 0) throw new InvalidOperationException("Stock cannot be negative");

        product.Stock = newStock;
        product.UpdatedUtc = DateTime.UtcNow;
        _db.StockMovements.Add(new StockMovement
        {
            ProductId = id,
            Type = delta >= 0 ? MovementType.In : MovementType.Out,
            Quantity = Math.Abs(delta),
            Reason = reason
        });
        await _db.SaveChangesAsync(ct);

        _cache.Remove($"product:{id}");
        _cache.RemoveByPrefix("products:");

        var dto = new ProductResponse(product.Id, product.Name, product.Sku, product.Price,
            product.Stock, product.CategoryId, "", product.IsActive, product.UpdatedUtc);

        await _hub.Clients.All.SendAsync("stockUpdated", new { productId = id, newStock }, ct);
        return dto;
    }
}