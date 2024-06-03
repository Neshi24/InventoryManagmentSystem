using Shared;

namespace OrderService.Repo
{
    public interface IOrderRepo
    {
        Task CreateOrder(Order order);
        Task CreateMissingItemHistory(MessageIds messageIds);
        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetAllOrders();
        Task<List<MessageIds>> GetAllOrdersHistory();
        Task UpdateOrder(Order order);
        Task DeleteOrder(int id);
        void RebuildDB();
    }
}