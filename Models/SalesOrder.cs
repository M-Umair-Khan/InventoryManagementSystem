using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class SalesOrder
    {
        [Key]
        public int SalesOrderID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "SO Number")]
        public string SONumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Required Date")]
        public DateTime? RequiredDate { get; set; }

        [Display(Name = "Shipped Date")]
        public DateTime? ShippedDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [StringLength(255)]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [StringLength(100)]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        // Navigation Properties
        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }
    }

    public class SalesOrderDetail
    {
        [Key]
        public int SODetailID { get; set; }

        [Required]
        public int SalesOrderID { get; set; }

        [Required]
        [Display(Name = "Product")]
        public int ProductID { get; set; }

        [Required]
        [Display(Name = "Quantity Ordered")]
        public int QuantityOrdered { get; set; }

        [Display(Name = "Quantity Shipped")]
        public int QuantityShipped { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Discount %")]
        public decimal DiscountPercent { get; set; } = 0;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Line Total")]
        public decimal LineTotal { get; private set; }

        // Navigation Properties
        public virtual SalesOrder SalesOrder { get; set; }
        public virtual Product Product { get; set; }
    }
}