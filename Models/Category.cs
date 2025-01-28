using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Blog_Website.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; } // Auto-incremental unique primary key

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Category name

        [MaxLength(255)]
        public string Description { get; set; } // Optional description of the category

        public DateTime CreatedDate { get; set; } // Created date
        public DateTime? UpdatedDate { get; set; } // Updated date (nullable)

        // Navigation property
       // public virtual ICollection<Blogg_Model> BlogPosts { get; set; } // Relation to BloggModel
    }
}
