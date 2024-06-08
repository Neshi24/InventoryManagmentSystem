using Microsoft.EntityFrameworkCore;
using Shared;

namespace InventoryService.Repo
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> ItemTable { get; set; }
        public DbSet<PerformanceMetrics> PerformanceMetrics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=db.db");
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<PerformanceMetrics>()
                .HasKey(p => p.Id);
            
        }

    }
}