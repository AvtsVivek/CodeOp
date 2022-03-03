using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.Features.Orders;
using Microsoft.eShopWeb.Infrastructure.Data;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.MediatorHandlers.OrdersTests
{
    public class GetMyOrders
    {
        [Fact]
        public async Task NotReturnNullIfOrdersArePresent()
        {
            var request = new GetMyOrdersQuery("SomeUserName");

            var options = new DbContextOptionsBuilder<CatalogContext>().
                UseInMemoryDatabase(databaseName: "Catalog")
                .Options;

            var handler = new GetMyOrdersHandler(new CatalogContext(options));
            var result = await handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
        }
    }
}
