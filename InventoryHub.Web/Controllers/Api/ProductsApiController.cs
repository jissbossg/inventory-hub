using InventoryHub.Web.Dtos;
using InventoryHub.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryHub.Web.Controllers.Api;

[ApiController]
[Route("api/products")]
public class ProductsApiController : ControllerBase
{
    private readonly IProductService _svc;
    public ProductsApiController(IProductService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductResponse>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null, [FromQuery] int? categoryId = null,
        CancellationToken ct = default)
    {
        var result = await _svc.GetPagedAsync(page, pageSize, search, categoryId, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> Get(int id, CancellationToken ct)
    {
        var dto = await _svc.GetByIdAsync(id, ct);
        return dto is null ? NotFound(new ApiError("not_found", $"Product {id} not found")) : Ok(dto);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> Categories(CancellationToken ct)
        => Ok(await _svc.GetCategoriesAsync(ct));

    [HttpPost("{id:int}/stock")]
    public async Task<ActionResult<ProductResponse>> UpdateStock(int id,
        [FromBody] StockChangeRequest req, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.UpdateStockAsync(id, req.Delta, req.Reason, ct);
            return Ok(dto);
        }
        catch (KeyNotFoundException) { return NotFound(new ApiError("not_found", "Product not found")); }
        catch (InvalidOperationException ex) { return BadRequest(new ApiError("invalid_stock", ex.Message)); }
    }
}

public record StockChangeRequest(int Delta, string? Reason);