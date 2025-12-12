using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class ProductSerials
    {
        [Key]
        public int SerialID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        [StringLength(100)]
        public string SerialNumber { get; set; }

        [StringLength(100)]
        public string LotNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ManufactureDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PurchasePrice { get; set; }

        [StringLength(20)]
        public string CurrentStatus { get; set; } = "InStock";

        public int? CurrentLocation { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        // Navigation Properties
        public virtual Product Product { get; set; }
        public virtual Bin Bin { get; set; }
    }
}