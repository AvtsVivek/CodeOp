using System;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Cohesion
{
    public class TestWithDelegate
    {
        [Fact]
        public async Task Test()
        {
            var sku = Guid.NewGuid().ToString();

            var sut = new GetProductBySkuHandlerDelegateExample(x => Task.FromResult(new Product { Sku = x }));
            var result = await sut.Handle(new GetProductBySkuRequest(sku), CancellationToken.None);

            result.Sku.ShouldBe(sku);
        }
    }
}