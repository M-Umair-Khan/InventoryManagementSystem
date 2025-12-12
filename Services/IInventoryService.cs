using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public interface IInventoryService
    {
        Task<List<Inventory>> GetInventoryAsync();
        Task<Inventory> GetInventoryByProductAndWarehouseAsync(int productId, int warehouseId);
        Task<Inventory> UpdateInventoryAsync(int productId, int warehouseId, int quantityChange, string transactionType);
        Task<List<Inventory>> GetLowStockInventoryAsync();
    }
}