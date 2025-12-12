using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class SupplierPerformance
    {
        [Key]
        public int PerformanceID { get; set; }

        [Required]
        public int SupplierID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EvaluationDate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal? OnTimeDeliveryRate { get; set; }

        [Column(TypeName = "decimal(3,1)")]
        [Range(0, 5)]
        public decimal? QualityRating { get; set; }

        [Column(TypeName = "decimal(3,1)")]
        [Range(0, 5)]
        public decimal? CostRating { get; set; }

        [Column(TypeName = "decimal(3,1)")]
        [Range(0, 5)]
        public decimal? CommunicationRating { get; set; }

        public int? TotalOrders { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? TotalSpent { get; set; }

        [StringLength(100)]
        public string EvaluatedBy { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        // Navigation Property
        public virtual Supplier Supplier { get; set; }
    }
}