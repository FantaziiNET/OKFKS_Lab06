using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    public class OrderServiceDbContext:DbContext
    {

        public OrderServiceDbContext(DbContextOptions<OrderServiceDbContext> options) : base(options)
        {
        }


        public DbSet<Order> Orders { get; set; }
    }
}
