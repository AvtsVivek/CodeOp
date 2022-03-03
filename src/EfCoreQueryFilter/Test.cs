using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MultiTenant
{
    public class Test
    {
        private readonly OrderDbContext.TenantFactory _factory;

        public Test()
        {
            var sc = new ServiceCollection();
            sc.AddSingleton<OrderDbContext.TenantFactory>(_ => OrderDbContext.Factory);
            var provider = sc.BuildServiceProvider();
            _factory = provider.GetRequiredService<OrderDbContext.TenantFactory>();
        }

        private async Task AddOrder(Token token)
        {
            await using var db = _factory(token);
            db.Orders.Add(new Order
            {
                TenantId = token.TenantId,
                OrderId = Guid.NewGuid()
            });
            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task TestFilter()
        {
            var tenant1 = new Token(Guid.NewGuid());
            await AddOrder(tenant1);

            var tenant2 = new Token(Guid.NewGuid());
            await AddOrder(tenant2);
            await AddOrder(tenant2);

            await using var db = _factory(tenant1);
            var tenant1Orders = await db.Orders.CountAsync();
            Assert.Equal(1, tenant1Orders);
        }
    }
}