using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class Warehouse
    {
        [Key]
        public int WarehouseID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Warehouse Code")]
        public string WarehouseCode { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Warehouse Name")]
        public string WarehouseName { get; set; }

        [StringLength(255)]
        public string Location { get; set; }

        [Display(Name = "Capacity")]
        public int? Capacity { get; set; }

        [StringLength(100)]
        [Display(Name = "Manager")]
        public string ManagerName { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Display(Name = "Active")]
        public bool? IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<Inventory> Inventories { get; set; }
    }
}