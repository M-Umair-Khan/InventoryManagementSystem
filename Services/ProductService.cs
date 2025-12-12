using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventories)
                .ThenInclude(i => i.Warehouse)
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventories)
                .ThenInclude(i => i.Warehouse)
                .FirstOrDefaultAsync(p => p.ProductID == id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedDate = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Initialize inventory for all warehouses
            var warehouses = await _context.Warehouses.Where(w => w.IsActive).ToListAsync();
            foreach (var warehouse in warehouses)
            {
                var inventory = new Inventory
                {
                    ProductID = product.ProductID,
                    WarehouseID = warehouse.WarehouseID,
                    QuantityOnHand = 0,
                    QuantityReserved = 0
                };
                _context.Inventory.Add(inventory);
            }

            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            product.UpdatedDate = DateTime.Now;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            product.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Product>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Inventories)
                .Where(p => p.IsActive &&
                    p.Inventories.Any(i => i.QuantityAvailable <= p.ReorderLevel))
                .ToListAsync();
        }
    }
}