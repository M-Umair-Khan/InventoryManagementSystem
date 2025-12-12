using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync();
        Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id);
        Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder order);
        Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder order);
        Task<bool> ReceivePurchaseOrderAsync(int orderId, List<PurchaseOrderDetail> receivedItems);
    }
}