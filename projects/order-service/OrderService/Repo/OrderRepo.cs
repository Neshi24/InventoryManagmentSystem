using CommonPackage;
using Microsoft.EntityFrameworkCore;
using OrderService.RabbitMQ;
using Shared;

namespace OrderService.Repo
{
    public class OrderRepo : IOrderRepo
    {
        private readonly DbContext _context;
        private readonly MessageClient _messageClient;

        public OrderRepo(DbContext context, MessageClient messageClient)
        {
            _context = context;
            _messageClient = messageClient;
        }

        public async Task CreateOrder(Order order)
        {
            try
            {
                _context.OrderTable.Add(order);
                await _context.SaveChangesAsync();
                var messageIds = new MessageIds
                {
                    OrderId = order.Id,
                    ItemsIds = order.ItemsIds
                };
                _messageClient.Publish(messageIds);
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to create order.", ex);
                throw;
            }
        }

        public async Task<Order> GetOrderById(int id)
        {
            try
            {
                return await _context.OrderTable.FindAsync(id);
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve order.", ex);
                throw;
            }
        }

        public async Task<List<Order>> GetAllOrders()
        {
            try
            {
                return await _context.OrderTable.ToListAsync();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve orders.", ex);
                throw;
            }
        }

        public async Task UpdateOrder(Order order)
        {
            try
            {
                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to update order.", ex);
                throw;
            }
        }

        public async Task DeleteOrder(int id)
        {
            try
            {
                var item = await _context.OrderTable.FindAsync(id);
                if (item != null)
                {
                    _context.OrderTable.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to delete order.", ex);
                throw;
            }
        }
        
        public void RebuildDB()
        {
            try
            {
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to rebuild database.", ex);
                throw;
            }
        }
    }
}
