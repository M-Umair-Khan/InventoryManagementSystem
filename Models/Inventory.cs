using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryID { get; set; }

        [Required]
        [Display(Name = "Product")]
        public int ProductID { get; set; }

        [Required]
        [Display(Name = "Warehouse")]
        public int WarehouseID { get; set; }

        [Display(Name = "Quantity on Hand")]
        public int QuantityOnHand { get; set; } = 0;

        [Display(Name = "Quantity Reserved")]
        public int? QuantityReserved { get; set; } = 0;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Quantity Available")]
        public int? QuantityAvailable { get; private set; }

        [Display(Name = "Last Stock Take")]
        public DateTime? LastStockTakeDate { get; set; }

        [Display(Name = "Last Restock")]
        public DateTime? LastRestockDate { get; set; }

        [Display(Name = "Next Reorder")]
        public DateTime? NextReorderDate { get; set; }

        // Navigation Properties
        public virtual Product Product { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}