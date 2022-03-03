using System;
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

            var product = new CatalogProduct("abc123", "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                "Leading software designers have recognized domain modeling and design as critical topics for at least twenty years, yet surprisingly little has been written about what needs to be done or how to do it. Although it has never been clearly formulated, a philosophy has developed as an undercurrent in the object community, which I call domain-driven design.",
                80,
                50);
            product.CanSetAvailable();

            modelBuilder.Entity<CatalogProduct>().HasData(product);

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
        public CatalogProduct()
        {

        }

        public CatalogProduct(string sku, string name, string description, decimal price, decimal cost)
        {
            Sku = sku;
            Name = name;
            Description = description;
            Price = price;
            Cost = cost;
        }

        public string Sku { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; private set; }
        public decimal Cost { get; private set; }
        public int QuantityOnHand { get; private set; }
        public bool ForSale { get; private set; }
        public bool FreeShipping { get; private set; }

        public void SetAvailable()
        {
            if (CanSetAvailable())
            {
                ForSale = true;
            }
        }

        public bool CanSetAvailable()
        {
            return ForSale == false && QuantityOnHand > 0;
        }

        public string ValidationError()
        {
            return CanSetAvailable()
                ? null
                : "Product is must be unavailable and Quantity greater than 0";
        }

        public bool CanSetUnavailable()
        {
            return ForSale;
        }

        public void InventoryAdjustment(int adjustment)
        {
            QuantityOnHand += adjustment;
            if (QuantityOnHand <= 0)
            {
                UnavailableForSale();
            }
        }

        public void IncreasePrice(decimal newPrice)
        {
            if (newPrice < Price)
            {
                throw new InvalidOperationException("New price must be greater than current price.");
            }

            Price = newPrice;
        }

        public void DecreasePrice(decimal newPrice)
        {
            if (newPrice > Price)
            {
                throw new InvalidOperationException("New price must be less than current price.");
            }

            Price = newPrice;

            if (Price <= 0)
            {
                UnavailableForSale();
            }
        }

        public void UnavailableForSale()
        {
            ForSale = false;
        }
    }

    public class CatalogProductImage
    {
        public int CatalogProductImageId { get; set; }
        public string Sku { get; set; }
        public string Url { get; set; }
    }
}