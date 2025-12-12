using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IInventoryService _inventoryService;

        public SalesOrderService(ApplicationDbContext context, IInventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        public async Task<List<SalesOrder>> GetAllSalesOrdersAsync()
        {
            return await _context.SalesOrders
                .Include(so => so.SalesOrderDetails)
                    .ThenInclude(sod => sod.Product)
                .OrderByDescending(so => so.OrderDate)
                .ToListAsync();
        }

        public async Task<SalesOrder> GetSalesOrderByIdAsync(int id)
        {
            return await _context.SalesOrders
                .Include(so => so.SalesOrderDetails)
                    .ThenInclude(sod => sod.Product)
                .FirstOrDefaultAsync(so => so.SalesOrderID == id);
        }

        public async Task<SalesOrder> CreateSalesOrderAsync(SalesOrder order)
        {
            // Generate SO Number
            order.SONumber = GenerateSONumber();
            order.CreatedBy = "System"; // Replace with actual user
            order.OrderDate = DateTime.Now;
            order.Status = "Pending";

            // Reserve inventory
            foreach (var detail in order.SalesOrderDetails)
            {
                await _inventoryService.UpdateInventoryAsync(
                    detail.ProductID,
                    1, // Default warehouse ID - should be configurable
                    detail.QuantityOrdered,
                    "RESERVE"
                );
            }

            _context.SalesOrders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<SalesOrder> UpdateSalesOrderAsync(SalesOrder order)
        {
            _context.SalesOrders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> FulfillSalesOrderAsync(int orderId)
        {
            var order = await GetSalesOrderByIdAsync(orderId);
            if (order == null) return false;

            // Check if all items are in stock
            foreach (var detail in order.SalesOrderDetails)
            {
                var inventory = await _inventoryService.GetInventoryByProductAndWarehouseAsync(detail.ProductID, 1);
                if (inventory.QuantityAvailable < detail.QuantityOrdered)
                {
                    return false; // Not enough stock
                }
            }

            // Process fulfillment
            foreach (var detail in order.SalesOrderDetails)
            {
                detail.QuantityShipped = detail.QuantityOrdered;

                // Update inventory
                await _inventoryService.UpdateInventoryAsync(
                    detail.ProductID,
                    1, // Default warehouse ID
                    detail.QuantityOrdered,
                    "SALE"
                );
            }

            order.Status = "Shipped";
            order.ShippedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateSONumber()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month.ToString("D2");
            var count = _context.SalesOrders.Count(so => so.OrderDate.HasValue && so.OrderDate.Value.Year == year && so.OrderDate.HasValue && so.OrderDate.Value.Month == DateTime.Now.Month) + 1;
                return $"SO-{year}{month}-{count.ToString("D4")}";
        }
    }
}