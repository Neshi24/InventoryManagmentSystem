using InventoryService.Models;

namespace InventoryService.Services
{
    public interface IItemService
    {
        Task CreateItem(ItemDto itemDto);
        Task<Item> GetItemById(int id);
        Task<List<Item>> GetAllItems();
        Task UpdateItem(int id, ItemDto itemDto);
        Task DeleteItem(int id);
    }
}