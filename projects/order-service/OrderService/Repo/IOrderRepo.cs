using Shared;

namespace OrderService.Repo
{
    public interface IOrderRepo
    {
        Task CreateOrder(Order order);
        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetAllOrders();
        Task UpdateOrder(Order order);
        Task DeleteOrder(int id);
        void RebuildDB();
    }
}