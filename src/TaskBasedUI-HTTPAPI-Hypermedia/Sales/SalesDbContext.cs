using Microsoft.EntityFrameworkCore;

namespace Sales
{
    public class SalesDbContext : DbContext
    {
        public DbSet<SalesProduct> Products { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=sales.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SalesProduct>().HasKey(product => product.Sku);

            modelBuilder.Entity<SalesProduct>().HasData(new SalesProduct
            {
                Sku = "abc123",
                Price = 85,
                ForSale = true,
                FreeShipping = false,
                QuantityOnHand = 25
            });
        }
    }

    public class SalesProduct
    {
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int QuantityOnHand { get; set; }
        public bool ForSale { get; set; }
        public bool FreeShipping { get; set; }

        public bool CanSetAvailable()
        {
            return ForSale == false && QuantityOnHand > 0;
        }

        public bool CanSetUnavailable()
        {
            return ForSale;
        }
    }
}