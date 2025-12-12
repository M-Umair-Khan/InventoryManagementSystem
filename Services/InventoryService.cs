using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Inventory>> GetInventoryAsync()
        {
//            return await _context.Inventories
            return await _context.Inventory
                .Include(i => i.Product)
                .Include(i => i.Warehouse)
                .OrderBy(i => i.Product.ProductName)
                .ToListAsync();
        }

        public async Task<Inventory> GetInventoryByProductAndWarehouseAsync(int productId, int warehouseId)
        {
//            return await _context.Inventories
            return await _context.Inventory
                .Include(i => i.Product)
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(i => i.ProductID == productId && i.WarehouseID == warehouseId);
        }

        public async Task<Inventory> UpdateInventoryAsync(int productId, int warehouseId, int quantityChange, string transactionType)
        {
            var inventory = await GetInventoryByProductAndWarehouseAsync(productId, warehouseId);

            if (inventory == null)
            {
                // Create new inventory record if it doesn't exist
                inventory = new Inventory
                {
                    ProductID = productId,
                    WarehouseID = warehouseId,
                    QuantityOnHand = 0,
                    QuantityReserved = 0
                };
  //              _context.Inventories.Add(inventory);
                _context.Inventory.Add(inventory);
            }

            // Update quantities based on transaction type
            switch (transactionType.ToUpper())
            {
                case "PURCHASE":
                case "ADJUST_IN":
                    inventory.QuantityOnHand += quantityChange;
                    inventory.LastRestockDate = DateTime.Now;
                    break;

                case "SALE":
                    inventory.QuantityReserved -= quantityChange;
                    inventory.QuantityOnHand -= quantityChange;
                    break;

                case "RESERVE":
                    inventory.QuantityReserved += quantityChange;
                    break;

                case "ADJUST_OUT":
                    inventory.QuantityOnHand -= quantityChange;
                    break;
            }

            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<List<Inventory>> GetLowStockInventoryAsync()
        {
//            return await _context.Inventories
            return await _context.Inventory
                .Include(i => i.Product)
                .Include(i => i.Warehouse)
                .Where(i => i.QuantityAvailable <= i.Product.ReorderLevel && i.Product.IsActive)
                .ToListAsync();
        }
    }
}