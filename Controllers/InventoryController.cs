using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
