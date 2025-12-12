using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class ProductBinning
    {
        [Key]
        public int BinningID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int BinID { get; set; }

        public int? QuantityInBin { get; set; } = 0;

        public int? MinStockLevel { get; set; } = 0;

        public int? MaxStockLevel { get; set; }

        public DateTime? LastCountDate { get; set; }

        // Navigation Properties
        public virtual Product Product { get; set; }
        public virtual Bin Bin { get; set; }
    }
}