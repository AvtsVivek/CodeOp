using Microsoft.EntityFrameworkCore;

namespace Catalog
{
    public class CatalogDbContext : DbContext
    {
        public DbSet<CatalogProduct> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=catalog.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CatalogProduct>().HasKey(product => product.Sku);
        }
    }

    public class CatalogProduct
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}