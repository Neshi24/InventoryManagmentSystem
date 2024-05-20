using AutoMapper;
using CommonPackage;
using OrderService.Repo;
using OpenTelemetry.Trace;
using Shared;

namespace OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;
        private readonly Tracer _tracer;

        public OrderService(IMapper mapper, IOrderRepo orderRepo, Tracer tracer)
        {
            _mapper = mapper;
            _orderRepo = orderRepo;
            _tracer = tracer;
        }

        public async Task CreateOrder(OrderDto orderDto)
        {
            using var activity = _tracer.StartActiveSpan("CreateOrder service");
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
        }

        public async Task<Order> GetOrderById(int id)
        {
            using var activity = _tracer.StartActiveSpan("GetOrderById service");
            try
            {
                return await _orderRepo.GetOrderById(id);
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve order.", ex);
                throw;
            }
        }

        public async Task<List<Order>> GetAllOrders()
        {
            using var activity = _tracer.StartActiveSpan("GetAllOrders service");
            try
            {
                return await _orderRepo.GetAllOrders();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve orders.", ex);
                throw;
            }
        }

        public async Task UpdateOrder(int id, OrderDto orderDto)
        {
            using var activity = _tracer.StartActiveSpan("UpdateOrder service");
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
        }

        public async Task DeleteOrder(int id)
        {
            using var activity = _tracer.StartActiveSpan("DeleteOrder service");
            try
            {
                await _orderRepo.DeleteOrder(id);
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to delete order.", ex);
                throw;
            }
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
