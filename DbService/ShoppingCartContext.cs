using Microsoft.EntityFrameworkCore;

namespace DbService
{
    public class ShoppingCartContext : DbContext
    {
        public ShoppingCartContext(DbContextOptions<ShoppingCartContext> options)
            :base(options) 
        {

        }
        public DbSet<Product> Product { get; set;}
        public DbSet<Purchase> Purchase { get; set;}
        public DbSet<PurchaseDetail> PurchaseDetail { get; set;}
        public DbSet<Category> Category { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Purchase>().ToTable("Purchase");
            modelBuilder.Entity<PurchaseDetail>().ToTable("PurchaseDetail");
            modelBuilder.Entity<Category>().ToTable("Category");
        }
    }
}