using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class InventoryAdjustment
    {
        [Key]
        public int AdjustmentID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int WarehouseID { get; set; }

        [Required]
        public int QuantityBefore { get; set; }

        [Required]
        public int QuantityAfter { get; set; }

//        public int QuantityDifference => QuantityAfter - QuantityBefore;

        [StringLength(20)]
        public string AdjustmentType { get; set; } // 'COUNT', 'DAMAGE', 'LOSS', 'GAIN'

        [StringLength(500)]
        public string Reason { get; set; }

        public DateTime AdjustmentDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string AdjustedBy { get; set; }

        [StringLength(100)]
        public string ApprovedBy { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        // Navigation Properties
        public virtual Product Product { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}