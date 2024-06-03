using AutoMapper;
using CommonPackage;
using OrderService.Repo;
using OpenTelemetry.Trace;
using Polly;
using Polly.Retry;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;
        private readonly Tracer _tracer;
        private readonly AsyncRetryPolicy _retryPolicy;

        public OrderService(IMapper mapper, IOrderRepo orderRepo, Tracer tracer)
        {
            _mapper = mapper;
            _orderRepo = orderRepo;
            _tracer = tracer;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task CreateOrder(OrderDto orderDto)
        {
            using var activity = _tracer.StartActiveSpan("CreateOrder service");
            await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var order = _mapper.Map<Order>(orderDto);
                    order.OrderDate = DateTime.Now;
                    await _orderRepo.CreateOrder(order);
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to create order.", ex);
                    throw;
                }
            });
        }

        public async Task CreateMissingItemHistory(MessageIdsDto messageIdsDto)
        {
            using var activity = _tracer.StartActiveSpan("CreateMissingItemHistory service");
            await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var missingItemHistory = _mapper.Map<MessageIds>(messageIdsDto);
                    await _orderRepo.CreateMissingItemHistory(missingItemHistory);
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to create MissingItemHistory.", ex);
                    throw;
                }
            });
        }

        public async Task<Order> GetOrderById(int id)
        {
            using var activity = _tracer.StartActiveSpan("GetOrderById service");
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    return await _orderRepo.GetOrderById(id);
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to retrieve order.", ex);
                    throw;
                }
            });
        }

        public async Task<List<Order>> GetAllOrders()
        {
            using var activity = _tracer.StartActiveSpan("GetAllOrders service");
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    return await _orderRepo.GetAllOrders();
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to retrieve orders.", ex);
                    throw;
                }
            });
        }

        public async Task<List<MessageIds>> GetAllOrdersHistory()
        {
            using var activity = _tracer.StartActiveSpan("GetAllOrdersHistory service");
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    return await _orderRepo.GetAllOrdersHistory();
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to retrieve ordersHistory.", ex);
                    throw;
                }
            });
        }
        
        public async Task UpdateOrder(int id, OrderDto orderDto)
        {
            using var activity = _tracer.StartActiveSpan("UpdateOrder service");
            await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var existingOrder = await _orderRepo.GetOrderById(id);
                    if (existingOrder == null)
                    {
                        throw new Exception("No such Order");
                    }

                    existingOrder.Address = orderDto.Address;
                    existingOrder.ItemsIds = orderDto.ItemsIds;

                    await _orderRepo.UpdateOrder(existingOrder);
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to update order.", ex);
                    throw new Exception("error: " + ex.Message);
                }
            });
        }

        public async Task DeleteOrder(int id)
        {
            using var activity = _tracer.StartActiveSpan("DeleteOrder service");
            await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    await _orderRepo.DeleteOrder(id);
                }
                catch (Exception ex)
                {
                    Monitoring.Log.Error("Unable to delete order.", ex);
                    throw;
                }
            });
        }

        public void RebuildDB()
        {
            using var activity = _tracer.StartActiveSpan("RebuildDB service");
            try
            {
                _orderRepo.RebuildDB();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to rebuild database.", ex);
                throw;
            }
        }
    }
}