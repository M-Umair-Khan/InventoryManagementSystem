using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IInventoryService _inventoryService;

        public PurchaseOrderService(ApplicationDbContext context, IInventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        public async Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync()
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderDetails)
                    .ThenInclude(pod => pod.Product)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderDetails)
                    .ThenInclude(pod => pod.Product)
                .FirstOrDefaultAsync(po => po.PurchaseOrderID == id);
        }

        public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder order)
        {
            // Generate PO Number
            order.PONumber = GeneratePONumber();
            order.CreatedBy = "System"; // Replace with actual user
            order.OrderDate = DateTime.Now;
            order.Status = "Pending";

            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder order)
        {
            _context.PurchaseOrders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> ReceivePurchaseOrderAsync(int orderId, List<PurchaseOrderDetail> receivedItems)
        {
            var order = await GetPurchaseOrderByIdAsync(orderId);
            if (order == null) return false;
            bool anyUpdates = false;
            foreach (var item in receivedItems)
            {
                var detail = order.PurchaseOrderDetails.FirstOrDefault(pod => pod.PODetailID == item.PODetailID);
                if (detail != null && item.QuantityReceived.HasValue)
                {
                    // Ensure quantity received is valid (non-negative and not exceeding ordered)
                    int quantityReceived = item.QuantityReceived.Value;
                    if (quantityReceived < 0)
                        quantityReceived = 0;
                    if (quantityReceived > detail.QuantityOrdered)
                        quantityReceived = detail.QuantityOrdered;

                    detail.QuantityReceived = item.QuantityReceived;
                    detail.ReceivedDate = DateTime.Now;

                    // Update inventory
                    await _inventoryService.UpdateInventoryAsync(
                        detail.ProductID,
                        1, // Default warehouse ID - should be configurable
                        quantityReceived,
                        "PURCHASE"
                    );
                    anyUpdates = true;
                }
            }
            if (!anyUpdates) return false;

            // Check if all items received
            if (order.PurchaseOrderDetails.All(pod => pod.QuantityReceived >= pod.QuantityOrdered))
            {
                order.Status = "Completed";
                order.ActualDeliveryDate = DateTime.Now;
            }
            else
            {
                order.Status = "Partially Received";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private string GeneratePONumber()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month.ToString("D2");
            var count = _context.PurchaseOrders.Count(po => po.OrderDate.HasValue && po.OrderDate.Value.Year == year && po.OrderDate.HasValue && po.OrderDate.Value.Month== DateTime.Now.Month) + 1;
                return $"PO-{year}{month}-{count.ToString("D4")}";
        }
    }
}