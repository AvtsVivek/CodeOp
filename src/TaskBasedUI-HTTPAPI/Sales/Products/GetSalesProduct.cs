using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales;
using Swashbuckle.AspNetCore.Annotations;

namespace Warehouse.Products
{
    [ApiController]
    public class GetSalesProductController : ControllerBase
    {
        private readonly SalesDbContext _db;

        public GetSalesProductController(SalesDbContext db)
        {
            _db = db;
        }

        [SwaggerOperation(
            OperationId = "GetSalesProduct",
            Tags = new[] { "Sales" })]
        [HttpGet("/sales/products/{sku}")]
        public async Task<IActionResult> GetSalesProduct([FromRoute] string sku)
        {
            var product = await _db.Products.SingleOrDefaultAsync(x => x.Sku == sku);
            if (product == null)
            {
                return NotFound();
            }

            var result = new ProductDto
            {
                Sku = sku,
                Price = product.Price,
                ForSale = product.ForSale,
                FreeShipping = product.FreeShipping
            };
            return Ok(result);
        }
    }

    public class ProductDto
    {
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public bool ForSale { get; set; }
        public bool FreeShipping { get; set; }
    }
}