using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_Website.Models
{
    // User Model
    public class User
    {
        //Primary Key
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public string FirstName { get; set; } 


        public string? LastName { get; set; }

        public string? UserType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Is_Deleted { get; set; } = false;
        // Navigation property for comments
        // public virtual ICollection<Comment> Comments { get; set; }
    }
}
