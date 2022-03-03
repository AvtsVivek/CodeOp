using System.Threading.Tasks;
using Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Sales.Products
{
    [ApiController]
    public class IncreasePriceController : ControllerBase
    {
        private readonly CatalogDbContext _db;

        public IncreasePriceController(CatalogDbContext db)
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
                return NotFound();
            }

            product.IncreasePrice(dto.Price);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

    public class PriceIncreaseDto
    {
        public decimal Price { get; set; }
    }
}