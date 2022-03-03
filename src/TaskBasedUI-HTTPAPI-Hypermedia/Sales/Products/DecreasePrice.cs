using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Sales.Products
{
    [ApiController]
    public class DecreasePriceController : ControllerBase
    {
        private readonly SalesDbContext _db;

        public DecreasePriceController(SalesDbContext db)
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
                product = new SalesProduct
                {
                    Sku = sku
                };
                _db.Products.Add(product);
            }

            if (dto.Price > product.Price)
            {
                return BadRequest("Price must be below current product price.");
            }

            product.Price = dto.Price;
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

    public class PriceDecreaseDto
    {
        public decimal Price { get; set; }
    }
}