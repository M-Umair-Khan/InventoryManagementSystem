using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    public class SalesOrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
