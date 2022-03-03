using Microsoft.EntityFrameworkCore;

namespace Purchasing
{
    public class PurchasingDbContext : DbContext
    {
        public DbSet<PurchasingProduct> Products { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=purchasing.db");
        }
    }

    public class PurchasingProduct
    {
        public string Sku { get; set; }
        public string Cost { get; set; }
    }
}