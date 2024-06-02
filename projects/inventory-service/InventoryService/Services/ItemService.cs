using InventoryService.Repo;
using AutoMapper;
using CommonPackage;
using InventoryService.RabbitMQ;
using OpenTelemetry.Trace;
using Shared;
using Polly;
using Polly.Retry;

namespace InventoryService.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepo _itemRepo;
        private readonly IMapper _mapper;
        private readonly Tracer _tracer;
        private readonly MessageClient _messageClient;
        private readonly AsyncRetryPolicy _retryPolicy;

        public ItemService(IItemRepo itemRepo, IMapper mapper, Tracer tracer, MessageClient messageClient)
        {
            _itemRepo = itemRepo;
            _mapper = mapper;
            _tracer = tracer;
            _messageClient = messageClient;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    3, 
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetryAsync: async (exception, timeSpan, retryCount, context) =>
                    {
                        Monitoring.Log.Error($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry. Context: {context.OperationKey}");
                        Console.WriteLine($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry. Context: {context.OperationKey}");
                        
                        await Task.CompletedTask;
                    });
        }

        public async Task CreateItem(ItemDto itemDto)
        {
            using var activity = _tracer.StartActiveSpan("CreateItem service");
            await _retryPolicy.ExecuteAsync(async () =>
            {
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
            });
        }

        public async Task<Item> GetItemById(int id)
        {
            using var activity = _tracer.StartActiveSpan("GetItemById service");
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    return await _itemRepo.GetItemById(id);
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to retrieve item.", ex);
                    throw;
                }
            });
        }

        public async Task<List<Item>> GetAllItems()
        {
            using var activity = _tracer.StartActiveSpan("GetAllItems service");
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    return await _itemRepo.GetAllItems();
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to retrieve items.", ex);
                    throw;
                }
            });
        }

        public async Task<List<Item>> GetItemsByIds(List<int> ids)
        {
            using var activity = _tracer.StartActiveSpan("GetItemsByIds service");
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    return await _itemRepo.GetItemsByIds(ids);
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to retrieve items based on id.", ex);
                    throw;
                }
            });
        }

        public async Task<List<int>> GetMissingIds(MessageIdsDto messageIdsDto)
        {
            using var activity = _tracer.StartActiveSpan("GetMissingIds service");
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var newMessageIds = new MessageIdsDto
                    {
                        ItemsIds = await _itemRepo.GetMissingIds(messageIdsDto.ItemsIds),
                        OrderId = messageIdsDto.OrderId
                    };

                    var queueName = "missingItems";
                    _messageClient.Publish(newMessageIds, queueName);

                    return newMessageIds.ItemsIds;
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to retrieve items based on id.", ex);
                    throw;
                }
            });
        }

        public async Task UpdateItem(int id, ItemDto itemDto)
        {
            using var activity = _tracer.StartActiveSpan("UpdateItem service");
            await _retryPolicy.ExecuteAsync(async () =>
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
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to update item.", ex);
                    throw;
                }
            });
        }

        public async Task DeleteItem(int id)
        {
            using var activity = _tracer.StartActiveSpan("DeleteItem service");
            await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    await _itemRepo.DeleteItem(id);
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to delete item.", ex);
                    throw;
                }
            });
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
