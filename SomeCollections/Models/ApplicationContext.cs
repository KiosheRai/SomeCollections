using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SomeCollections.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Item>().Property(u => u.LikeCount).HasDefaultValue(0);
            modelBuilder.Entity<Collection>().Property(u => u.CountItems).HasDefaultValue(0);

            modelBuilder.Entity<Tag>().HasData(
                new Tag[]
                {
                    new Tag{ Id = 1, Name = "Books" },
                    new Tag{ Id = 2, Name = "Alcohol" },
                    new Tag{ Id = 3, Name = "Pets" },
                    new Tag{ Id = 4, Name = "Cars" },
                    new Tag{ Id = 5, Name = "News" },
                    new Tag{ Id = 6, Name = "Games" },
                    new Tag{ Id = 7, Name = "Places" },
                });
        }
    }
}
