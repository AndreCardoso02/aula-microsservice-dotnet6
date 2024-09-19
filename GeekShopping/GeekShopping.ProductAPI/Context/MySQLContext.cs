using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Context
{
    public class MySQLContext : DbContext
    {
        public MySQLContext() {}

        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) {}
    }
}
