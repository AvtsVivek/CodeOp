using System;
using Microsoft.EntityFrameworkCore;

namespace MultiTenant
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid TenantId { get; set; }
    }

    public class OrderDbContext : DbContext
    {
        private readonly Token _token;

        public DbSet<Order> Orders { get; set; }

        public OrderDbContext(Token token)
        {
            _token = token;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasKey(x => x.OrderId);
            modelBuilder.Entity<Order>().HasQueryFilter(b => b.TenantId == _token.TenantId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("MultiTenant");
        }

        public delegate OrderDbContext TenantFactory(Token token);

        public static OrderDbContext Factory(Token token)
        {
            return new OrderDbContext(token);
        }

    }
}