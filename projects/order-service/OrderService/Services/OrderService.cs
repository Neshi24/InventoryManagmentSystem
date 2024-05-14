using AutoMapper;
using OrderService.Repo;
using Shared;

namespace OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;

        public OrderService(IMapper mapper, IOrderRepo orderRepo)
        {
            _mapper = mapper;
            _orderRepo = orderRepo;
        }

        public async Task CreateOrder(OrderDto orderDto)
        {
            var Order = _mapper.Map<Order>(orderDto);
            Order.OrderDate = DateTime.Now;
            await _orderRepo.CreateOrder(Order);
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _orderRepo.GetOrderById(id);
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _orderRepo.GetAllOrders();
        }

        public async Task UpdateOrder(int id, OrderDto orderDto)
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
            catch (Exception e)
            {

                throw new Exception("error: " + e);
            }
        }


        public async Task DeleteOrder(int id)
        {
            await _orderRepo.DeleteOrder(id);
        }
        
        public void RebuildDB()
        {
            _orderRepo.RebuildDB();
        }
    }

}