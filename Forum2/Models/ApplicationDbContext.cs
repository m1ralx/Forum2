using System.Data.Entity;
using Forum.MySql;
using Microsoft.AspNet.Identity.EntityFramework;
using Forum2.Models;

namespace Forum.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        static ApplicationDbContext()
        {
            Database.SetInitializer(new MySqlInitializer());
        }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ForumThread>()
                .HasRequired(x => x.Owner)
                .WithMany(x => x.Threads)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Message>()
                .HasRequired(x => x.Owner)
                .WithMany(x => x.Messages)
                .WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Board> Boards { get; set; }
        public DbSet<ForumThread> Threads { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}