using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Category Name")]
        public string? CategoryName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public int? ParentCategoryID { get; set; }

        [Display(Name = "Active")]
        public bool? IsActive { get; set; } = true;

       // public DateTime CreatedDate { get; set; } = DateTime.Now;
      //  public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Category> ChildCategories { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}