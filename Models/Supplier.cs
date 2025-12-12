using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [StringLength(100)]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [Display(Name = "Active")]
        public bool? IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}