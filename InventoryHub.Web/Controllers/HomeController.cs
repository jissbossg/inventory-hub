using Microsoft.AspNetCore.Mvc;

namespace InventoryHub.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}