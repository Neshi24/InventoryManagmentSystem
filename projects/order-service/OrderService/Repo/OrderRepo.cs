using Microsoft.EntityFrameworkCore;
using Shared;

namespace OrderService.Repo
{
    public class OrderRepo : IOrderRepo
    {
        private readonly DbContext _context;

        public OrderRepo(DbContext context)
        {
            _context = context;
        }

        public async Task CreateOrder(Order order)
        {
            _context.OrderTable.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _context.OrderTable.FindAsync(id);
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _context.OrderTable.ToListAsync();
        }

        public async Task UpdateOrder(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrder(int id)
        {
            var item = await _context.OrderTable.FindAsync(id);
            if (item != null)
            {
                _context.OrderTable.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
        
        public void RebuildDB()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
    }
}