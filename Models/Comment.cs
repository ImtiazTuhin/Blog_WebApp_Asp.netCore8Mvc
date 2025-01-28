using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Blog_Website.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; } // Auto-incremental unique primary key

        [Required]
        public string CommenterName { get; set; }

        [Required]
        [ForeignKey(nameof(Blog))]
        public int PostId { get; set; } // Foreign key for Blogg_Model.Id

        [Required]
        public string Text { get; set; }

        //// Foreign key to the User table
        //[Required]
        //[ForeignKey(nameof(User))]
        //public int UserId { get; set; } // Foreign key for User.Id

        public int? UserId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } // Created date

        public DateTime? UpdatedDate { get; set; } // Updated date (nullable)

        // Navigation properties
       // public virtual User User { get; set; } // Navigation property for User model
        public virtual Blogg_Model Blog { get; set; } // Navigation property for Blogg_Model
    }
}
