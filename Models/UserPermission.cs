using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class UserPermission
    {
        [Key]
        public int PermissionID { get; set; }

        [Required]
        public int RoleID { get; set; }

        [Required]
        [StringLength(50)]
        public string Module { get; set; }

        public bool? CanView { get; set; } = false;
        public bool? CanCreate { get; set; } = false;
        public bool? CanEdit { get; set; } = false;
        public bool? CanDelete { get; set; } = false;
        public bool? CanApprove { get; set; } = false;

        // Navigation Property
        public virtual UserRole UserRole { get; set; }
    }
}