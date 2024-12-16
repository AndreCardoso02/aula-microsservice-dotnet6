using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Model.Base
{
    public class MySQLContext : DbContext
    {
        public MySQLContext() { }

        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) { }

        public DbSet<OrderDetail> CartDetails { get; set; }
        public DbSet<OrderHeader> CartHeaders { get; set; }
    }
}
