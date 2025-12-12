using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public interface ISalesOrderService
    {
        Task<List<SalesOrder>> GetAllSalesOrdersAsync();
        Task<SalesOrder> GetSalesOrderByIdAsync(int id);
        Task<SalesOrder> CreateSalesOrderAsync(SalesOrder order);
        Task<SalesOrder> UpdateSalesOrderAsync(SalesOrder order);
        Task<bool> FulfillSalesOrderAsync(int orderId);
    }
}