using System.Collections.Generic;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalWarehouses { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int LowStockItems { get; set; }
        public int PendingPurchaseOrders { get; set; }
        public int PendingSalesOrders { get; set; }

        public List<Product> LowStockProducts { get; set; }
        public List<SalesOrder> RecentSales { get; set; }
        public List<PurchaseOrder> RecentPurchases { get; set; }

        public ChartData SalesChart { get; set; }
        public ChartData InventoryChart { get; set; }
    }

    public class ChartData
    {
        public List<string> Labels { get; set; }
        public List<decimal> Values { get; set; }
    }
}