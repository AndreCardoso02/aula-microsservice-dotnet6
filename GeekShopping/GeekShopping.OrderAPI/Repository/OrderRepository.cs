using GeekShopping.OrderAPI.Model.Base;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<MySQLContext> _options;

        public OrderRepository(DbContextOptions<MySQLContext> options)
        {
            _options=options;
        }

        public async Task<bool> AddOrder(OrderHeader header)
        {
            if (header == null) return false;
            await using var _db = new MySQLContext(_options);
            _db.OrderHeaders.Add(header);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderPaymentStatus(long orderHeaderId, bool paid)
        {
            await using var _db = new MySQLContext(_options);
            var header = await _db.OrderHeaders.FirstOrDefaultAsync(x => x.Id == orderHeaderId);
            if (header != null)
            {
                header.PaymentStatus = paid;
                await _db.SaveChangesAsync();
            }
        }
    }
}
