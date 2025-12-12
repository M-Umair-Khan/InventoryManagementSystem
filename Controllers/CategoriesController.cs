using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.Models;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.ParentCategories = await _categoryService.GetAllCategoriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ParentCategories = await _categoryService.GetAllCategoriesAsync();
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.ParentCategories = await _categoryService.GetAllCategoriesAsync();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ParentCategories = await _categoryService.GetAllCategoriesAsync();
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}