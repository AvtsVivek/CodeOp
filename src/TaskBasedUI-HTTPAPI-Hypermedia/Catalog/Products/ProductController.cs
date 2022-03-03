using System.Collections.Generic;
using System.Linq;
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
        private readonly IUrlHelper _urlHelper;

        public ProductController(CatalogDbContext db, IUrlHelper urlHelper)
        {
            _db = db;
            _urlHelper = urlHelper;
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

            var result = new ProductResponse
            {
                Sku = sku,
                Name = product.Name,
                Description = product.Description,
            };

            //result.Links.Add(new Link("GetCatalogProductImages", _urlHelper.Action("GetImages", new { sku })));
            //result.Actions.Add(new Action("UpdateCatalogProduct", _urlHelper.Action("Update", new { sku })));

            return Ok(result);
        }

        [SwaggerOperation(
            OperationId = "GetCatalogProductImages",
            Tags = new[] { "Catalog" })]
        [HttpGet("/catalog/products/{sku}/images")]
        public async Task<IActionResult> GetImages([FromRoute] string sku)
        {
            var images = await _db.ProductImages.Where(x => x.Sku == sku).ToArrayAsync();

            var result = new ProductImagesResponse
            {
                Sku = sku,
                Images = images.Select(x => x.Url).ToArray()
            };

            return Ok(result);
        }

        [SwaggerOperation(
            OperationId = "UpdateCatalogProduct",
            Tags = new[] { "Catalog" })]
        [HttpPut("/catalog/products/{sku}", Name = "Update")]
        public async Task<IActionResult> Update([FromRoute] string sku, [FromBody] ProductUpdateRequest updateRequest)
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

            product.Description = updateRequest.Description;
            product.Name = updateRequest.Name;
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

    public class ProductResponse
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //public IList<Link> Links { get; set; } = new List<Link>();
        //public IList<Action> Actions { get; set; } = new List<Action>();
    }

    public class ProductImagesResponse
    {
        public string Sku { get; set; }
        public string[] Images { get; set; }
    }

    public class ProductUpdateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public record Link(string Rel, string Href);
    public record Action(string Name, string Href);


}