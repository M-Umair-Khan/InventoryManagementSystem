using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class SystemConfiguration
    {
        [Key]
        public int ConfigID { get; set; }

        [Required]
        [StringLength(100)]
        public string ConfigKey { get; set; }

        [StringLength(500)]
        public string ConfigValue { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(50)]
        public string DataType { get; set; } = "String";

        public DateTime? LastModified { get; set; } = DateTime.Now;
    }
}