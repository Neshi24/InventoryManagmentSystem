using InventoryService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Repo
{
    public class ItemRepo : IItemRepo
    {
        private readonly DbContext _context;

        public ItemRepo(DbContext context)
        {
            _context = context;
        }

        public async Task CreateItem(Item item)
        {
            _context.ItemTable.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<Item> GetItemById(int id)
        {
            return await _context.ItemTable.FindAsync(id);
        }

        public async Task<List<Item>> GetAllItems()
        {
            return await _context.ItemTable.ToListAsync();
        }

        public async Task UpdateItem(Item item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItem(int id)
        {
            var item = await _context.ItemTable.FindAsync(id);
            if (item != null)
            {
                _context.ItemTable.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
        
        public void RebuildDB()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
    }
}