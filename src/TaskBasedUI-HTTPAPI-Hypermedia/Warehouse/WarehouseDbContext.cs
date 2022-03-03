using Microsoft.EntityFrameworkCore;

namespace Warehouse
{
    public class WarehouseDbContext : DbContext
    {
        public DbSet<WarehouseProduct> Products { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=warehouse.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WarehouseProduct>().HasKey(product => product.Sku);

            modelBuilder.Entity<WarehouseProduct>().HasData(new WarehouseProduct
            {
                Sku = "abc123",
                QuantityOnHand = 25
            });
        }
    }

    public class WarehouseProduct
    {
        public string Sku { get; set; }
        public int QuantityOnHand { get; set; }
    }
}