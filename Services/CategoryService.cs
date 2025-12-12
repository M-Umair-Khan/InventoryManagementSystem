using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.ChildCategories)
                .Include(c => c.Products)
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.ChildCategories)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryID == id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            category.CreatedDate = DateTime.Now;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            category.UpdatedDate = DateTime.Now;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null) return false;

            // Check if category has products
            if (category.Products != null && category.Products.Any(p => p.IsActive))
            {
                return false; // Cannot delete category with active products
            }

            // Check if category has child categories
            var hasActiveChildCategories = await _context.Categories
                .AnyAsync(c => c.ParentCategoryID == id && c.IsActive);

            if (hasActiveChildCategories)
            {
                return false; // Cannot delete category with active child categories
            }

            // Soft delete
            category.IsActive = false;
            category.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetParentCategoryOptionsAsync(int? excludeCategoryId = null)
        {
            var query = _context.Categories
                .Where(c => c.IsActive);

            if (excludeCategoryId.HasValue)
            {
                // Exclude the category itself and its descendants
                var excludedIds = await GetCategoryAndDescendantIdsAsync(excludeCategoryId.Value);
                query = query.Where(c => !excludedIds.Contains(c.CategoryID));
            }

            return await query
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        private async Task<List<int>> GetCategoryAndDescendantIdsAsync(int categoryId)
        {
            var allIds = new List<int> { categoryId };
            await GetDescendantIdsRecursiveAsync(categoryId, allIds);
            return allIds;
        }

        private async Task GetDescendantIdsRecursiveAsync(int parentId, List<int> ids)
        {
            var childIds = await _context.Categories
                .Where(c => c.ParentCategoryID == parentId && c.IsActive)
                .Select(c => c.CategoryID)
                .ToListAsync();

            if (childIds.Any())
            {
                ids.AddRange(childIds);
                foreach (var childId in childIds)
                {
                    await GetDescendantIdsRecursiveAsync(childId, ids);
                }
            }
        }

        public async Task<List<Category>> GetChildCategoriesAsync(int parentCategoryId)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryID == parentCategoryId && c.IsActive)
                .Include(c => c.Products)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<List<CategoryWithProductCount>> GetCategoriesWithProductCountAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .Where(c => c.IsActive)
                .Select(c => new CategoryWithProductCount
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    ParentCategoryID = c.ParentCategoryID,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.CategoryName : null,
                    ProductCount = c.Products.Count(p => p.IsActive),
                    IsActive = c.IsActive
                })
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<bool> CategoryNameExistsAsync(string categoryName, int? excludeCategoryId = null)
        {
            var query = _context.Categories
                .Where(c => c.CategoryName.ToLower() == categoryName.ToLower());

            if (excludeCategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryID != excludeCategoryId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Get category hierarchy for tree view
        /// </summary>
        public async Task<List<CategoryHierarchy>> GetCategoryHierarchyAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .Include(c => c.Products)
                .ToListAsync();

            var rootCategories = categories
                .Where(c => c.ParentCategoryID == null)
                .OrderBy(c => c.CategoryName)
                .ToList();

            var hierarchy = new List<CategoryHierarchy>();

            foreach (var rootCategory in rootCategories)
            {
                hierarchy.Add(BuildCategoryHierarchy(rootCategory, categories));
            }

            return hierarchy;
        }

        private CategoryHierarchy BuildCategoryHierarchy(Category category, List<Category> allCategories)
        {
            var hierarchy = new CategoryHierarchy
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                ProductCount = category.Products.Count(p => p.IsActive),
                Description = category.Description,
                Children = new List<CategoryHierarchy>()
            };

            var childCategories = allCategories
                .Where(c => c.ParentCategoryID == category.CategoryID)
                .OrderBy(c => c.CategoryName)
                .ToList();

            foreach (var child in childCategories)
            {
                hierarchy.Children.Add(BuildCategoryHierarchy(child, allCategories));
            }

            return hierarchy;
        }
    }

    /// <summary>
    /// Hierarchy structure for category tree view
    /// </summary>
    public class CategoryHierarchy
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int ProductCount { get; set; }
        public List<CategoryHierarchy> Children { get; set; }
    }
}