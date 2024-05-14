using InventoryService.Models;
using InventoryService.Repo;
using AutoMapper;

namespace InventoryService.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepo _itemRepo;
        private readonly IMapper _mapper;

        public ItemService(IItemRepo itemRepo, IMapper mapper)
        {
            _itemRepo = itemRepo;
            _mapper = mapper;
        }

        public async Task CreateItem(ItemDto itemDto)
        {
            var item = _mapper.Map<Item>(itemDto);
            await _itemRepo.CreateItem(item);
        }

        public async Task<Item> GetItemById(int id)
        {
            return await _itemRepo.GetItemById(id);
        }

        public async Task<List<Item>> GetAllItems()
        {
            return await _itemRepo.GetAllItems();
        }

        public async Task UpdateItem(int id, ItemDto itemDto)
        {
            try
            {
                var existingItem = await _itemRepo.GetItemById(id);
                if (existingItem == null)
                {
                    throw new Exception("No such item");
                }

                existingItem.Name = itemDto.Name;
                existingItem.Quantity = itemDto.Quantity;

                await _itemRepo.UpdateItem(existingItem);

            }
            catch (Exception e)
            {

                throw new Exception("error: " + e);
            }
        }


        public async Task DeleteItem(int id)
        {
            await _itemRepo.DeleteItem(id);
        }
        
        public void RebuildDB()
        {
            _itemRepo.RebuildDB();
        }
    }

}