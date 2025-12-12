using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class InventoryAuditLog
    {
        [Key]
        public int AuditID { get; set; }

        [StringLength(100)]
        public string TableName { get; set; }

        public int? RecordID { get; set; }

        [StringLength(20)]
        public string ChangeType { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        [StringLength(100)]
        public string ChangedBy { get; set; }

        public DateTime? ChangeDate { get; set; } = DateTime.Now;
    }
}