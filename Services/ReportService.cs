using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var dashboard = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(p => p.IsActive == true),
                TotalCategories = await _context.Categories.CountAsync(c => c.IsActive == true),
                TotalSuppliers = await _context.Suppliers.CountAsync(s => s.IsActive == true),
                TotalWarehouses = await _context.Warehouses.CountAsync(w => w.IsActive == true),

                TotalInventoryValue = await _context.Inventory
                    .Include(i => i.Product)
                    .SumAsync(i => i.QuantityOnHand * i.Product.CostPrice),

                LowStockItems = await _context.Inventory
                    .Include(i => i.Product)
                    .CountAsync(i => i.QuantityAvailable <= i.Product.ReorderLevel && i.Product.IsActive == true),

                PendingPurchaseOrders = await _context.PurchaseOrders
                    .CountAsync(po => po.Status == "Pending"),

                PendingSalesOrders = await _context.SalesOrders
                    .CountAsync(so => so.Status == "Pending"),

                LowStockProducts = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Inventories)
                    .Where(p => p.IsActive == true &&
                        p.Inventories.Any(i => i.QuantityAvailable <= p.ReorderLevel))
                    .Take(10)
                    .ToListAsync(),

                RecentSales = await _context.SalesOrders
                    .Include(so => so.SalesOrderDetails)
                    .OrderByDescending(so => so.OrderDate)
                    .Take(5)
                    .ToListAsync(),

                RecentPurchases = await _context.PurchaseOrders
                    .Include(po => po.PurchaseOrderDetails)
                    .OrderByDescending(po => po.OrderDate)
                    .Take(5)
                    .ToListAsync()
            };

            // Generate chart data (simplified - would need real data in production)
            dashboard.SalesChart = new ChartData
            {
                Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                Values = new List<decimal> { 15000, 18000, 22000, 19000, 25000, 28000 }
            };

            dashboard.InventoryChart = new ChartData
            {
                Labels = await _context.Categories
                 .Where(c => c.IsActive == true)
                 .Select(c => c.CategoryName ?? "Uncategorized") // Add null-coalescing
                .ToListAsync(),
                    Values = await _context.Products
                .Where(p => p.IsActive == true)
                .GroupBy(p => p.CategoryID)
                .Select(g => (decimal)g.Count())
                .ToListAsync()
            };

            return dashboard;
        }

        public async Task<List<ReorderRecommendation>> GetReorderRecommendationsAsync()
        {
            return await _context.Products
                .Include(p => p.Inventories)
                .Include(p => p.Category)
                .Where(p => p.IsActive == true)
                 .Select(p => new ReorderRecommendation
                 {
                     ProductID = p.ProductID,
                     ProductCode = p.ProductCode,
                     ProductName = p.ProductName,
                     CurrentStock = p.Inventories != null ?
                p.Inventories.Sum(i => i.QuantityAvailable ?? 0) : 0,
                     ReorderLevel = p.ReorderLevel ?? 0,
                     RecommendedQuantity = p.ReorderQuantity ?? 0,
                     EstimatedCost = (p.ReorderQuantity ?? 0) * p.CostPrice
                 })
                .Where(r => r.CurrentStock <= r.ReorderLevel)
                .ToListAsync();
        }

        public async Task<List<SalesReport>> GetSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.SalesOrderDetails
                .Include(sod => sod.SalesOrder)
                .Include(sod => sod.Product)
                .Where(sod => sod.SalesOrder.OrderDate >= startDate && sod.SalesOrder.OrderDate <= endDate)
                .GroupBy(sod => new { sod.ProductID, sod.Product.ProductName })
                .Select(g => new SalesReport
                {
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(x => x.QuantityShipped),
                    TotalRevenue = g.Sum(x => x.LineTotal),
                    Profit = g.Sum(x => x.LineTotal) - g.Sum(x => x.QuantityShipped * x.Product.CostPrice)
                })
                .OrderByDescending(r => r.TotalRevenue)
                .ToListAsync();
        }

        public async Task<List<InventoryValuationReport>> GetInventoryValuationReportAsync()
        {
            return await _context.Inventory
                .Include(i => i.Product)
                    .ThenInclude(p => p.Category)
                .GroupBy(i => new { i.ProductID, i.Product.ProductCode, i.Product.ProductName, i.Product.Category.CategoryName })
                .Select(g => new InventoryValuationReport
                {
                    ProductCode = g.Key.ProductCode,
                    ProductName = g.Key.ProductName,
                    Category = g.Key.CategoryName,
                    QuantityOnHand = g.Sum(x => x.QuantityOnHand),
                    UnitCost = g.Average(x => x.Product.CostPrice),
                    TotalValue = g.Sum(x => x.QuantityOnHand * x.Product.CostPrice)
                })
                .OrderByDescending(r => r.TotalValue)
                .ToListAsync();
        }

        public async Task<List<DeadStockReport>> GetDeadStockReportAsync()
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var today = DateTime.Now;

            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventories)
                .Where(p => p.IsActive == true)
                .Select(p => new DeadStockReport
                {
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Category = p.Category != null ? p.Category.CategoryName : "Uncategorized",
                    QuantityOnHand = p.Inventories.Sum(i => (int?)i.QuantityOnHand) ?? 0,
                    LastMovementDate = _context.StockTransactions
                        .Where(st => st.ProductID == p.ProductID)
                        .Max(st => (DateTime?)st.TransactionDate) ??
                        p.CreatedDate ?? DateTime.MinValue,
                    DaysInStock = (int)(today -
                        (p.Inventories.Any() ?
                            (p.Inventories.First().LastRestockDate ?? p.CreatedDate ?? today) :
                            p.CreatedDate ?? today)).TotalDays
                })
                .Where(r => r.LastMovementDate < sixMonthsAgo && r.QuantityOnHand > 0)
                .OrderBy(r => r.LastMovementDate)
                .ToListAsync();
        }
        public async Task<List<SupplierPerformanceReport>> GetSupplierPerformanceReportAsync()
        {
            return await _context.Suppliers
                .Include(s => s.PurchaseOrders)
                .Where(s => s.IsActive == true)
                .Select(s => new SupplierPerformanceReport
                {
                    SupplierName = s.SupplierName,
                    TotalOrders = s.PurchaseOrders != null ? s.PurchaseOrders.Count : 0,
                    OnTimeDeliveries = s.PurchaseOrders != null ?
                        s.PurchaseOrders.Count(po =>
                            po.ActualDeliveryDate.HasValue &&
                            po.ExpectedDeliveryDate.HasValue &&
                            po.ActualDeliveryDate <= po.ExpectedDeliveryDate &&
                            po.Status == "Completed") : 0,
                    OnTimeDeliveryRate = s.PurchaseOrders != null && s.PurchaseOrders.Count > 0 ?
                        (decimal)s.PurchaseOrders.Count(po =>
                            po.ActualDeliveryDate.HasValue &&
                            po.ExpectedDeliveryDate.HasValue &&
                            po.ActualDeliveryDate <= po.ExpectedDeliveryDate &&
                            po.Status == "Completed") / s.PurchaseOrders.Count * 100 : 0,
                    AverageLeadTime = s.PurchaseOrders != null && s.PurchaseOrders.Any(po =>
                        po.ActualDeliveryDate.HasValue && po.OrderDate.HasValue) ?
                        (decimal)s.PurchaseOrders
                            .Where(po => po.ActualDeliveryDate.HasValue && po.OrderDate.HasValue)
                            .Average(po => (po.ActualDeliveryDate.Value - po.OrderDate.Value).TotalDays) : 0,
                    QualityRating = 4.5m // Placeholder - would need quality metrics
                })
                .ToListAsync();
        }

        public async Task<List<SalesReport>> GetMonthlySalesReportAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return await GetSalesReportAsync(startDate, endDate);
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            return await _context.Inventory
                .Include(i => i.Product)
                .SumAsync(i => i.QuantityOnHand * i.Product.CostPrice);
        }

        public async Task<int> GetTotalActiveProductsAsync()
        {
            return await _context.Products.CountAsync(p => p.IsActive == true);
        }

        public async Task<int> GetTotalOutOfStockItemsAsync()
        {
            return await _context.Inventory
                .Include(i => i.Product)
                .CountAsync(i => i.QuantityAvailable <= 0 && i.Product.IsActive == true);
        }

        public async Task<Dictionary<string, int>> GetProductCountByCategoryAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive == true && p.Category != null)
                .GroupBy(p => p.Category.CategoryName)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);
        }

        public async Task<List<SalesReport>> GetTopSellingProductsAsync(int topN = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.SalesOrderDetails
                .Include(sod => sod.SalesOrder)
                .Include(sod => sod.Product)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(sod => sod.SalesOrder.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(sod => sod.SalesOrder.OrderDate <= endDate.Value);

            return await query
                .GroupBy(sod => new { sod.ProductID, sod.Product.ProductName })
                .Select(g => new SalesReport
                {
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(x => x.QuantityShipped),
                    TotalRevenue = g.Sum(x => x.LineTotal),
                    Profit = g.Sum(x => x.LineTotal) - g.Sum(x => x.QuantityShipped * x.Product.CostPrice)
                })
                .OrderByDescending(r => r.QuantitySold)
                .Take(topN)
                .ToListAsync();
        }
    }
}