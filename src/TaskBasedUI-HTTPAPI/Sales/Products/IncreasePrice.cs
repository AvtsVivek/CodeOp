using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Sales.Products
{
    [ApiController]
    public class IncreasePriceController : ControllerBase
    {
        private readonly SalesDbContext _db;

        public IncreasePriceController(SalesDbContext db)
        {
            _db = db;
            _db.Database.EnsureCreated();
        }

        [SwaggerOperation(
            OperationId = "IncreasePrice",
            Tags = new[] { "Sales" })]
        [HttpPost("/sales/products/{sku}/increasePrice")]
        public async Task<IActionResult> IncreasePrice([FromRoute] string sku, [FromBody] PriceIncreaseDto dto)
        {
            var product = await _db.Products.SingleOrDefaultAsync(x => x.Sku == sku);
            if (product == null)
            {
                product = new SalesProduct
                {
                    Sku = sku
                };
                _db.Products.Add(product);
            }

            product.Price = dto.Price;
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

    public class PriceIncreaseDto
    {
        public decimal Price { get; set; }
    }
}