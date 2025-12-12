using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class ScheduledReport
    {
        [Key]
        public int ScheduleID { get; set; }

        [Required]
        [StringLength(100)]
        public string ReportName { get; set; }

        [Required]
        [StringLength(50)]
        public string ReportType { get; set; }

        [StringLength(20)]
        public string Frequency { get; set; } // Daily, Weekly, Monthly, Quarterly

        public string Parameters { get; set; }

        public string Recipients { get; set; }

        public DateTime? LastRunDate { get; set; }

        public DateTime? NextRunDate { get; set; }

        public bool? IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }
}