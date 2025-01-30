using BL.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi007.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "ryan123", JobTitle = "Administrator" },
                new User { Id = 2, Username = "john", Password = "umer123", JobTitle = "Developer" }
            );
        }
    }
}
