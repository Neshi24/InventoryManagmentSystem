using Shared;

namespace OrderService.Services
{
    public interface IOrderService
    {
        Task CreateOrder(OrderDto orderDto);
        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetAllOrders();
        Task<Order> AssignItemsIds(int id, List<int> ids);
        Task UpdateOrder(int id, OrderDto orderDto);
        Task DeleteOrder(int id);
        void RebuildDB();
    }
}