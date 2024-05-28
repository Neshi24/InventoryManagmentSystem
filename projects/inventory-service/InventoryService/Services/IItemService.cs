

using Shared;

namespace InventoryService.Services
{
    public interface IItemService
    {
        Task CreateItem(ItemDto itemDto);
        Task<Item> GetItemById(int id);
        Task<List<Item>> GetAllItems();
        Task<List<Item>> GetItemsByIds(List<int> ids);
        Task<List<int>> GetMissingIds(MessageIdsDto messageIdsDto);
        Task UpdateItem(int id, ItemDto itemDto);
        Task DeleteItem(int id);
        void RebuildDB();
    }
}