using InventoryManagementSystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public interface IReportService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
        Task<List<ReorderRecommendation>> GetReorderRecommendationsAsync();
        Task<List<SalesReport>> GetSalesReportAsync(DateTime startDate, DateTime endDate);
        Task<List<InventoryValuationReport>> GetInventoryValuationReportAsync();
        Task<List<DeadStockReport>> GetDeadStockReportAsync();
        Task<List<SupplierPerformanceReport>> GetSupplierPerformanceReportAsync();

        // Additional methods
        Task<List<SalesReport>> GetMonthlySalesReportAsync(int year, int month);
        Task<decimal> GetTotalInventoryValueAsync();
        Task<int> GetTotalActiveProductsAsync();
        Task<int> GetTotalOutOfStockItemsAsync();
        Task<Dictionary<string, int>> GetProductCountByCategoryAsync();
        Task<List<SalesReport>> GetTopSellingProductsAsync(int topN = 10, DateTime? startDate = null, DateTime? endDate = null);
    }
    // Add this to IReportService.cs or create separate file
    // 报告模型类（需要创建）
    public class ReorderRecommendation
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public int RecommendedQuantity { get; set; }
        public decimal EstimatedCost { get; set; }
    }

    public class SalesReport
    {
        public DateTime Date { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal Profit { get; set; }
    }

    public class InventoryValuationReport
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int QuantityOnHand { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class DeadStockReport
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int QuantityOnHand { get; set; }
        public DateTime LastMovementDate { get; set; }
        public int DaysInStock { get; set; }
    }

    public class SupplierPerformanceReport
    {
        public string SupplierName { get; set; }
        public int TotalOrders { get; set; }
        public int OnTimeDeliveries { get; set; }
        public decimal OnTimeDeliveryRate { get; set; }
        public decimal AverageLeadTime { get; set; }
        public decimal QualityRating { get; set; }
    }
}