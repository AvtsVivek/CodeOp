using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Xunit;

namespace Cohesion
{
    public class TestWithInterface
    {
        [Fact]
        public async Task Test()
        {
            var sku = Guid.NewGuid().ToString();

            var mockProductService = new Mock<IProductService>();
            mockProductService
                .Setup(x => x.GetProductBySku(sku))
                .Returns(Task.FromResult(new Product()
                {
                    Sku = sku
                }));

            var sut = new GetProductBySkuHandler(mockProductService.Object);
            var result = await sut.Handle(new GetProductBySkuRequest(sku), CancellationToken.None);

            result.Sku.ShouldBe(sku);
        }
    }
}