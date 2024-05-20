using InventoryService.Repo;
using AutoMapper;
using CommonPackage;
using InventoryService.RabbitMQ;
using OpenTelemetry.Trace;
using Shared;

namespace InventoryService.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepo _itemRepo;
        private readonly IMapper _mapper;
        private readonly Tracer _tracer;
        private readonly MessageClient _messageClient;

        public ItemService(IItemRepo itemRepo, IMapper mapper, Tracer tracer, MessageClient messageClient)
        {
            _itemRepo = itemRepo;
            _mapper = mapper;
            _tracer = tracer;
            _messageClient = messageClient;
        }

        public async Task CreateItem(ItemDto itemDto)
        {
            using var activity = _tracer.StartActiveSpan("CreateItem service");
            try
            {
                var item = _mapper.Map<Item>(itemDto);
                await _itemRepo.CreateItem(item);
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to create item.", ex);
                throw;
            }
        }

        public async Task<Item> GetItemById(int id)
        {
            using var activity = _tracer.StartActiveSpan("GetItemById service");
            try
            {
                return await _itemRepo.GetItemById(id);
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve item.", ex);
                throw;
            }
        }

        public async Task<List<Item>> GetAllItems()
        {
            using var activity = _tracer.StartActiveSpan("GetAllItems service");
            try
            {
                return await _itemRepo.GetAllItems();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve items.", ex);
                throw;
            }
        }
        
        public async Task<List<Item>> GetItemsByIds(List<int> ids)
        {
            using var activity = _tracer.StartActiveSpan("GetItemsByIds service");
            try
            {
                return await _itemRepo.GetItemsByIds(ids);
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve items basing on id.", ex);
                throw;
            }
        }

        
        public async Task<List<int>> GetMissingIds(MessageIds messageIds)
        {
            using var activity = _tracer.StartActiveSpan("GetItemsByIds service");
            try
            {
                var newMessageIds = new MessageIds
                {
                    ItemsIds = await _itemRepo.GetMissingIds(messageIds.ItemsIds),
                    OrderId = messageIds.OrderId
                };
                _messageClient.Publish(newMessageIds);
                return newMessageIds.ItemsIds;
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve items basing on id.", ex);
                throw;
            }
        }
        
        
        public async Task UpdateItem(int id, ItemDto itemDto)
        {
            using var activity = _tracer.StartActiveSpan("UpdateItem service");
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
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to update item.", ex);
                throw;
            }
        }

        public async Task DeleteItem(int id)
        {
            using var activity = _tracer.StartActiveSpan("DeleteItem service");
            try
            {
                await _itemRepo.DeleteItem(id);
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to delete item.", ex);
                throw;
            }
        }

        public void RebuildDB()
        {
            using var activity = _tracer.StartActiveSpan("RebuildDB service");
            try
            {
                _itemRepo.RebuildDB();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to rebuild database.", ex);
                throw;
            }
        }
    }
}
