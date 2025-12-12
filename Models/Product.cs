using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Display(Name = "Supplier")]
        public int? SupplierID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }

        [Display(Name = "Reorder Level")]
        public int? ReorderLevel { get; set; } = 10;

        [Display(Name = "Reorder Quantity")]
        public int? ReorderQuantity { get; set; } = 50;

        [Display(Name = "Active")]
        public bool? IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        [Column(TypeName = "decimal(8,2)")]
        public decimal? Weight { get; set; }

        [StringLength(50)]
        public string Dimensions { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        // Navigation Properties
        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }
    }
}