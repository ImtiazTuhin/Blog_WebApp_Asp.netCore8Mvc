using Blog_Website.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog_Website.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Blogg_Model> Bloggers { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ChangePasswordViewModel> ChangePwd { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure User entity
            //builder.Entity<User>(entity =>
            //{
            //    entity.HasKey(u => u.Id);
            //    entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            //    entity.Property(u => u.Password).IsRequired();
            //    entity.Property(u => u.FirstName).HasMaxLength(100);
            //    entity.Property(u => u.LastName).HasMaxLength(100);
            //});

            //// Configure Blogger_Model entity
            //builder.Entity<Blogg_Model>(entity =>
            //{
            //    entity.HasKey(b => b.Id);
            //    entity.Property(b => b.Name).IsRequired().HasMaxLength(100);
            //    entity.Property(b => b.BlogTitle).HasMaxLength(200);
            //});
        }
    }
}
