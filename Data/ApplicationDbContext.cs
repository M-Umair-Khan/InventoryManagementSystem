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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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