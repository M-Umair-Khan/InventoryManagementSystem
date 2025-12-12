using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int PurchaseOrderID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "PO Number")]
        public string PONumber { get; set; }

        [Required]
        [Display(Name = "Supplier")]
        public int SupplierID { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Expected Delivery")]
        public DateTime? ExpectedDeliveryDate { get; set; }

        [Display(Name = "Actual Delivery")]
        public DateTime? ActualDeliveryDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [StringLength(100)]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [StringLength(100)]
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        // Navigation Properties
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    }

    public class PurchaseOrderDetail
    {
        [Key]
        public int PODetailID { get; set; }

        [Required]
        public int PurchaseOrderID { get; set; }

        [Required]
        [Display(Name = "Product")]
        public int ProductID { get; set; }

        [Required]
        [Display(Name = "Quantity Ordered")]
        public int QuantityOrdered { get; set; }

        [Display(Name = "Quantity Received")]
        public int QuantityReceived { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Line Total")]
        public decimal LineTotal { get; private set; }

        [Display(Name = "Received Date")]
        public DateTime? ReceivedDate { get; set; }

        // Navigation Properties
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual Product Product { get; set; }
    }
}