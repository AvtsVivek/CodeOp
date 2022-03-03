using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Products
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CatalogDbContext _db;

        public ProductController(CatalogDbContext db)
        {
            _db = db;
            _db.Database.EnsureCreated();
        }

        [SwaggerOperation(
            OperationId = "GetCatalogProduct",
            Tags = new[] { "Catalog" })]
        [HttpGet("/catalog/products/{sku}")]
        public async Task<IActionResult> Get([FromRoute] string sku)
        {
            var product = await _db.Products.SingleOrDefaultAsync(x => x.Sku == sku);
            if (product == null)
            {
                return NotFound();
            }

            var result = new ProductDto
            {
                Sku = sku,
                Name = product.Name,
                Description = product.Description
            };
            return Ok(result);
        }

        [SwaggerOperation(
            OperationId = "UpdateCatalogProduct",
            Tags = new[] { "Catalog" })]
        [HttpPut("/catalog/products/{sku}", Name = "Update")]
        public async Task<IActionResult> Update([FromRoute] string sku, [FromBody] ProductDto productDto)
        {
            var product = await _db.Products.SingleOrDefaultAsync(x => x.Sku == sku);
            if (product == null)
            {
                product = new CatalogProduct
                {
                    Sku = sku
                };
                _db.Products.Add(product);
            }

            product.Description = productDto.Description;
            product.Name = productDto.Name;
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

    public class ProductDto
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}