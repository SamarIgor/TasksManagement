using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using task.Models;

namespace task.Data
{
    public class TaskContext : IdentityDbContext<AccountUser>
    {
        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Tasks> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Comment>().ToTable("Comment");
            modelBuilder.Entity<Priority>().ToTable("Priority");
            modelBuilder.Entity<Tasks>().ToTable("Tasks");
        }
    }
}
