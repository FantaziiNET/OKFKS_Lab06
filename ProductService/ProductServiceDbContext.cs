using Microsoft.EntityFrameworkCore;

namespace ProductService
{
    public class ProductServiceDbContext:DbContext
    {

        public ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options) : base(options)
        {
        }


        public DbSet<Product> Products { get; set; }
    }
}