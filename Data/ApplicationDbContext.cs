using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
//        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<InventoryAdjustment> InventoryAdjustments { get; set; }
        public DbSet<Bin> Bins { get; set; }
        public DbSet<ProductBinning> ProductBinnings { get; set; }
        public DbSet<ProductSerials> ProductSerials { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
        public DbSet<SystemConfiguration> SystemConfigurations { get; set; }
        public DbSet<SupplierPerformance> SupplierPerformances { get; set; }
        public DbSet<ScheduledReport> ScheduledReports { get; set; }
        public DbSet<InventoryValuation> InventoryValuations { get; set; }
        public DbSet<InventoryAuditLog> InventoryAuditLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Bin>()
                .HasIndex(b => new { b.WarehouseID, b.BinCode })
                .IsUnique();
            // Product Configuration
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductCode)
                .IsUnique();

            // Inventory Configuration
            modelBuilder.Entity<Inventory>()
                .HasIndex(i => new { i.ProductID, i.WarehouseID })
                .IsUnique();

            modelBuilder.Entity<Inventory>()
                .Property(i => i.QuantityAvailable)
                .HasComputedColumnSql("[QuantityOnHand] - [QuantityReserved]", stored: true);

            // Purchase Order Configuration
            modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(po => po.PONumber)
                .IsUnique();

            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(pod => pod.LineTotal)
                .HasComputedColumnSql("[QuantityOrdered] * [UnitCost]");

            // Sales Order Configuration
            modelBuilder.Entity<SalesOrder>()
                .HasIndex(so => so.SONumber)
                .IsUnique();

            modelBuilder.Entity<SalesOrderDetail>()
                .Property(sod => sod.LineTotal)
                .HasComputedColumnSql("[QuantityOrdered] * [UnitPrice] * (1 - [DiscountPercent]/100)");

            // Relationships
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .HasForeignKey(c => c.ParentCategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierID)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}