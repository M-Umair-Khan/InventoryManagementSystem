using System.Diagnostics;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.Models.ViewModels;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IReportService _reportService;

        public HomeController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = await _reportService.GetDashboardDataAsync();
            return View(dashboardData);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}