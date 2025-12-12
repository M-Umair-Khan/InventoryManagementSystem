// Create new file: Models/Bin.cs
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class Bin
    {
        [Key]
        public int BinID { get; set; }

        [Required]
        [StringLength(50)]
        public string BinCode { get; set; }

        [Required]
        public int WarehouseID { get; set; }

        [StringLength(50)]
        public string Zone { get; set; }

        [StringLength(20)]
        public string Aisle { get; set; }

        [StringLength(20)]
        public string Shelf { get; set; }

        [StringLength(20)]
        public string Position { get; set; }

        public int? Capacity { get; set; }
        public int? CurrentOccupancy { get; set; }
        public bool? IsActive { get; set; }

        // Navigation Properties
        public virtual Warehouse Warehouse { get; set; }
    }
}
