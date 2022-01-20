using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SomeCollections.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Like> Likes { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Item>().Property(u => u.Likes).HasDefaultValue(0);
            modelBuilder.Entity<Collection>().Property(u => u.CountItems).HasDefaultValue(0);
        }
    }
}
