using Microsoft.EntityFrameworkCore;

namespace Catalog
{
    public class CatalogDbContext : DbContext
    {
        public DbSet<CatalogProduct> Products { get; set; }
        public DbSet<CatalogProductImage> ProductImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=catalog.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CatalogProduct>().HasKey(product => product.Sku);
            modelBuilder.Entity<CatalogProductImage>().HasKey(product => product.CatalogProductImageId);

            modelBuilder.Entity<CatalogProduct>().HasData(new CatalogProduct
            {
                Sku = "abc123",
                Name = "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                Description = "Leading software designers have recognized domain modeling and design as critical topics for at least twenty years, yet surprisingly little has been written about what needs to be done or how to do it. Although it has never been clearly formulated, a philosophy has developed as an undercurrent in the object community, which I call domain-driven design.",
            });

            modelBuilder.Entity<CatalogProductImage>().HasData(new CatalogProductImage
                {
                    CatalogProductImageId = 1,
                    Sku = "abc123",
                    Url = "https://images-na.ssl-images-amazon.com/images/I/51sZW87slRL.jpg"
                },
                new CatalogProductImage
                {
                    CatalogProductImageId = 2,
                    Sku = "abc123",
                    Url = "https://images-na.ssl-images-amazon.com/images/I/81Tx5LAzr8L.jpg"
                });
        }
    }

    public class CatalogProduct
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CatalogProductImage
    {
        public int CatalogProductImageId { get; set; }
        public string Sku { get; set; }
        public string Url { get; set; }
    }
}