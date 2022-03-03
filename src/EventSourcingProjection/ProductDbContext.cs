using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Demo
{
    public class Product
    {
        public string Sku { get; set; }
        public int Received { get; set; }
        public int Shipped { get; set; }
    }

    public class ProductDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasKey(x => x.Sku);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseInMemoryDatabase("Demo");
        }
    }
}