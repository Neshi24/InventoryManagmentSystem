using CommonPackage;
using Microsoft.EntityFrameworkCore;
using Shared;

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
            try
            {
                _context.ItemTable.Add(item);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to create item.", ex);
                throw;
            }
        }

        public async Task<Item> GetItemById(int id)
        {
            try
            {
                return await _context.ItemTable.FindAsync(id);
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve item.", ex);
                throw;
            }
        }

        public async Task<List<Item>> GetAllItems()
        {
            try
            {
                return await _context.ItemTable.ToListAsync();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve items.", ex);
                throw;
            }
        }
        
        public async Task<List<Item>> GetItemsByIds(List<int> ids)
        {
            try
            {
                return await _context.ItemTable.Where(item => ids.Contains(item.Id)).ToListAsync();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve items by ids.", ex);
                throw;
            }
        }
        
        public async Task<List<int>> GetMissingIds(List<int> ids)
        {
            try
            {
                var existingIds = await _context.ItemTable
                    .Where(item => ids.Contains(item.Id))
                    .Select(item => item.Id)
                    .ToListAsync();

                var missingIds = ids.Except(existingIds).ToList();

                return missingIds;
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve missing ids.", ex);
                throw;
            }
        }

        public async Task UpdateItem(Item item)
        {
            try
            {
                _context.Entry(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to update item.", ex);
                throw;
            }
        }

        public async Task DeleteItem(int id)
        {
            try
            {
                var item = await _context.ItemTable.FindAsync(id);
                if (item != null)
                {
                    _context.ItemTable.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to delete item.", ex);
                throw;
            }
        }

        public void RebuildDB()
        {
            try
            {
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to rebuild database.", ex);
                throw;
            }
        }
    }
}
