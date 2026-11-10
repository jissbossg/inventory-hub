using InventoryHub.Web.Services;
using InventoryHub.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryHub.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _svc;
    public ProductsController(IProductService svc) => _svc = svc;

    public async Task<IActionResult> Index(int page = 1, string? search = null, int? categoryId = null, CancellationToken ct = default)
    {
        var data = await _svc.GetPagedAsync(page, 6, search, categoryId, ct);
        var cats = await _svc.GetCategoriesAsync(ct);
        var vm = new ProductListViewModel
        {
            Data = data,
            Categories = cats.ToList(),
            Search = search,
            CategoryId = categoryId
        };
        return View(vm);
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var product = await _svc.GetByIdAsync(id, ct);
        if (product is null) return NotFound();
        return View(product);
    }
}