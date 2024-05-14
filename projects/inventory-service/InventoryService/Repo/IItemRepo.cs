using InventoryService.Models;


namespace InventoryService.Repo
{
    public interface IItemRepo
    {
        Task CreateItem(Item item);
        Task<Item> GetItemById(int id);
        Task<List<Item>> GetAllItems();
        Task UpdateItem(Item item);
        Task DeleteItem(int id);
    }
}