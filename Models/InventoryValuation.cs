using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class InventoryValuation
    {
        [Key]
        public int ValuationID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ValuationDate { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int WarehouseID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitCost { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalValue { get; set; }

        [StringLength(20)]
        public string ValuationMethod { get; set; } = "FIFO";

        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual Product Product { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}