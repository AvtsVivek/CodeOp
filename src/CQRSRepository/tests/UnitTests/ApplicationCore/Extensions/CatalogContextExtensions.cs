using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.UnitTests.Builders;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Extensions
{
    public class CatalogContextExtensions
    {
        private readonly CatalogContext _catalogContext;
        private OrderBuilder OrderBuilder { get; } = new();

        public CatalogContextExtensions()
        {
            var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(databaseName: "TestCatalog")
                .Options;
            _catalogContext = new CatalogContext(dbOptions);
        }

        [Fact]
        public async Task CustomerOrdersWithItems()
        {
            //Arrange
            var itemOneUnitPrice = 5.50m;
            var itemOneUnits = 2;
            var itemTwoUnitPrice = 7.50m;
            var itemTwoUnits = 5;

            var firstOrder = OrderBuilder.WithBuyerId("ABC123");
            _catalogContext.Orders.Add(firstOrder);

            var secondOrderItems = new List<OrderItem>();
            secondOrderItems.Add(new OrderItem(OrderBuilder.TestCatalogItemOrdered, itemOneUnitPrice, itemOneUnits));
            secondOrderItems.Add(new OrderItem(OrderBuilder.TestCatalogItemOrdered, itemTwoUnitPrice, itemTwoUnits));
            var secondOrder = OrderBuilder.WithItems(secondOrderItems);
            _catalogContext.Orders.Add(secondOrder);
            int secondOrderId = secondOrder.Id;

            await _catalogContext.SaveChangesAsync();

            var orders = await _catalogContext.CustomerOrdersWithItems(secondOrder.BuyerId).ToArrayAsync();

            //Assert
            Assert.Single(orders);
            var customerOrder = orders.Single();
            Assert.Equal(secondOrderId, customerOrder.Id);
            Assert.Equal(secondOrder.OrderItems.Count, customerOrder.OrderItems.Count);
            Assert.Equal(1, customerOrder.OrderItems.Count(x => x.UnitPrice == itemOneUnitPrice));
            Assert.Equal(1, customerOrder.OrderItems.Count(x => x.UnitPrice == itemTwoUnitPrice));
            Assert.Equal(itemOneUnits, customerOrder.OrderItems.Single(x => x.UnitPrice == itemOneUnitPrice).Units);
            Assert.Equal(itemTwoUnits, customerOrder.OrderItems.Single(x => x.UnitPrice == itemTwoUnitPrice).Units);
        }
    }
}