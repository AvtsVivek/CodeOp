using System;
using System.Threading.Tasks;
using Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;

namespace Sales.Products
{
    [ApiController]
    public class DecreasePriceController : ControllerBase
    {
        private readonly CatalogDbContext _db;

        public DecreasePriceController(CatalogDbContext db)
        {
            _db = db;
            _db.Database.EnsureCreated();
        }

        [SwaggerOperation(
            OperationId = "DecreasePrice",
            Tags = new[] { "Sales" })]
        [HttpPost("/sales/products/{sku}/decreasePrice")]
        public async Task<IActionResult> DecreasePrice([FromRoute] string sku, [FromBody] PriceDecreaseDto dto)
        {
            var product = await _db.Products.SingleOrDefaultAsync(x => x.Sku == sku);
            if (product == null)
            {
                throw new InvalidOperationException();
            }

            product.DecreasePrice(dto.Price);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

    public class PriceDecreaseDto
    {
        public decimal Price { get; set; }
    }
}