using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class UnitOfMeasure
    {
        [Key]
        public int UOMID { get; set; }

        [Required]
        [StringLength(10)]
        public string UOMCode { get; set; }

        [Required]
        [StringLength(50)]
        public string UOMName { get; set; }

        [StringLength(100)]
        public string Description { get; set; }
    }
}