using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<List<Product>> GetLowStockProductsAsync();
    }
}