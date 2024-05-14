using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Repo;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options)
        : base(options) {
    }
    public DbSet<Item> ItemTable { get; set; }
    
}