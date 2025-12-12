using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
