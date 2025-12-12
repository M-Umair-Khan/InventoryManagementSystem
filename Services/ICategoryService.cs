using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public interface ICategoryService
    {
        /// Get all active categories with their parent/child relationships
        Task<List<Category>> GetAllCategoriesAsync();
        /// Get category by ID with its parent and child categories
        Task<Category> GetCategoryByIdAsync(int id);
        /// Create a new category
        Task<Category> CreateCategoryAsync(Category category);
        /// Update an existing category
        Task<Category> UpdateCategoryAsync(Category category);
        /// Soft delete a category (set IsActive to false)
        Task<bool> DeleteCategoryAsync(int id);
        /// Get categories that can be used as parent categories (excluding self)
        Task<List<Category>> GetParentCategoryOptionsAsync(int? excludeCategoryId = null);
        /// Get all child categories for a parent category
        Task<List<Category>> GetChildCategoriesAsync(int parentCategoryId);
        /// Get categories with product count
        Task<List<CategoryWithProductCount>> GetCategoriesWithProductCountAsync();
        /// Check if category name already exists
        Task<bool> CategoryNameExistsAsync(string categoryName, int? excludeCategoryId = null);
    }
    /// View model for categories with product count
    public class CategoryWithProductCount
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = null;
        public string Description { get; set; } = null;
        public int? ParentCategoryID { get; set; }
        public string ParentCategoryName { get; set; } = null;
        public int ProductCount { get; set; }
        public bool IsActive { get; set; }
    }
}