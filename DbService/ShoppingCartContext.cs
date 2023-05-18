using Microsoft.EntityFrameworkCore;

namespace DbService
{
    public class ShoppingCartContext : DbContext
    {
        public ShoppingCartContext(DbContextOptions<ShoppingCartContext> options)
            :base(options) 
        {

        }
        public DbSet<Product> Products { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Product");
        }
    }
}