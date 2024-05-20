
using Shared;

namespace InventoryService.Repo
{
    public interface IItemRepo
    {
        Task CreateItem(Item item);
        Task<Item> GetItemById(int id);
        Task<List<Item>> GetAllItems();
        Task<List<Item>> GetItemsByIds(List<int> ids);
        Task<List<int>> GetMissingIds(List<int> ids);
        Task UpdateItem(Item item);
        Task DeleteItem(int id);
        void RebuildDB();
    }
}