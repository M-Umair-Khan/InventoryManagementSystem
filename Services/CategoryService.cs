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
                .Where(c => c.IsActive == true) // Handle nullable boolean
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
            // Ensure IsActive is set (nullable in DB, non-null in service)
            category.IsActive = true;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
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

            // Check if category has active products
            if (category.Products != null && category.Products.Any(p => p.IsActive == true))
            {
                return false; // Cannot delete category with active products
            }

            // Check if category has active child categories
            var hasActiveChildCategories = await _context.Categories
                .AnyAsync(c => c.ParentCategoryID == id && c.IsActive == true);

            if (hasActiveChildCategories)
            {
                return false; // Cannot delete category with active child categories
            }

            // Soft delete
            category.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetParentCategoryOptionsAsync(int? excludeCategoryId = null)
        {
            var query = _context.Categories
                .Where(c => c.IsActive == true);

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
                .Where(c => c.ParentCategoryID == parentId && c.IsActive == true)
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
                .Where(c => c.ParentCategoryID == parentCategoryId && c.IsActive == true)
                .Include(c => c.Products)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<List<CategoryWithProductCount>> GetCategoriesWithProductCountAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .Where(c => c.IsActive == true)
                .Select(c => new CategoryWithProductCount
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    ParentCategoryID = c.ParentCategoryID,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.CategoryName : null,
                    ProductCount = c.Products.Count(p => p.IsActive == true),
                    IsActive = c.IsActive ?? false
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
                .Where(c => c.IsActive == true)
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
                ProductCount = category.Products.Count(p => p.IsActive == true),
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

        /// <summary>
        /// Get all active categories (simple list without includes)
        /// </summary>
        public async Task<List<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive == true)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        /// <summary>
        /// Get categories that have no parent (root categories)
        /// </summary>
        public async Task<List<Category>> GetRootCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryID == null && c.IsActive == true)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        /// <summary>
        /// Check if category can be deleted (no products or child categories)
        /// </summary>
        public async Task<bool> CanDeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryID == categoryId);

            if (category == null) return false;

            // Check for active products
            if (category.Products != null && category.Products.Any(p => p.IsActive == true))
            {
                return false;
            }

            // Check for active child categories
            var hasActiveChildren = await _context.Categories
                .AnyAsync(c => c.ParentCategoryID == categoryId && c.IsActive == true);

            return !hasActiveChildren;
        }

        /// <summary>
        /// Get category with all descendants
        /// </summary>
        public async Task<List<Category>> GetCategoryWithDescendantsAsync(int categoryId)
        {
            var category = await GetCategoryByIdAsync(categoryId);
            if (category == null) return new List<Category>();

            var allCategories = new List<Category> { category };
            await GetDescendantsRecursiveAsync(categoryId, allCategories);
            return allCategories;
        }

        private async Task GetDescendantsRecursiveAsync(int parentId, List<Category> categories)
        {
            var childCategories = await _context.Categories
                .Where(c => c.ParentCategoryID == parentId && c.IsActive == true)
                .ToListAsync();

            if (childCategories.Any())
            {
                categories.AddRange(childCategories);
                foreach (var child in childCategories)
                {
                    await GetDescendantsRecursiveAsync(child.CategoryID, categories);
                }
            }
        }

        /// <summary>
        /// Get breadcrumb path for a category
        /// </summary>
        public async Task<List<Category>> GetCategoryBreadcrumbAsync(int categoryId)
        {
            var breadcrumb = new List<Category>();
            await BuildBreadcrumbRecursiveAsync(categoryId, breadcrumb);
            return breadcrumb.Reverse<Category>().ToList();
        }

        private async Task BuildBreadcrumbRecursiveAsync(int categoryId, List<Category> breadcrumb)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryID == categoryId);

            if (category != null)
            {
                breadcrumb.Add(category);
                if (category.ParentCategoryID.HasValue)
                {
                    await BuildBreadcrumbRecursiveAsync(category.ParentCategoryID.Value, breadcrumb);
                }
            }
        }

        /// <summary>
        /// Count total categories (active and inactive)
        /// </summary>
        public async Task<int> GetTotalCategoriesCountAsync()
        {
            return await _context.Categories.CountAsync();
        }

        /// <summary>
        /// Count active categories
        /// </summary>
        public async Task<int> GetActiveCategoriesCountAsync()
        {
            return await _context.Categories.CountAsync(c => c.IsActive == true);
        }

        /// <summary>
        /// Get categories that have products
        /// </summary>
        public async Task<List<Category>> GetCategoriesWithProductsAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive == true && c.Products.Any(p => p.IsActive == true))
                .Include(c => c.Products)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
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