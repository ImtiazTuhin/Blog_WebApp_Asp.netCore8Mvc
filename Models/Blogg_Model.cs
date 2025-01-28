using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Blog_Website.Models
{
    public class Blogg_Model
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary key with auto-increment

        [Required]
        public string Name { get; set; } // Required field

        [Required]
        public string BlogTitle { get; set; } // Required field

        public string? Content { get; set; }
        public string? ImagePath { get; set; }

        // Foreign key to User table
        [ForeignKey(nameof(User))]
        public int UserId { get; set; } // Foreign key for User.Id (not nullable)
        public virtual User? User { get; set; } // Navigation property

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string? Category { get; set; }

        public bool Is_Deleted { get; set; } = false;

    }
}
