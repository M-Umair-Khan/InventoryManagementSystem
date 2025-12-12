using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class StockTransaction
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int WarehouseID { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; } // 'IN', 'OUT', 'ADJUST'

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [StringLength(100)]
        public string ReferenceNumber { get; set; } // PO#, SO#, Adjustment#

        [StringLength(500)]
        public string Remarks { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string CreatedBy { get; set; }

        // Navigation Properties
        public virtual Product Product { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}