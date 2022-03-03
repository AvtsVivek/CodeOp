using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly IUrlHelper _urlHelper;

        public GetSalesProductController(SalesDbContext db, IUrlHelper urlHelper)
        {
            _db = db;
            _db.Database.EnsureCreated();
            _urlHelper = urlHelper;
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

            result.Actions.Add(new Action("IncreasePrice", _urlHelper.Action("IncreasePrice", "IncreasePrice", new { sku })));
            result.Actions.Add(new Action("DecreasePrice", _urlHelper.Action("DecreasePrice", "DecreasePrice", new { sku })));

            if (product.CanSetAvailable())
            {
                result.Actions.Add(new Action("AvailableForSale", _urlHelper.Action("AvailableForSale", "AvailableForSale", new { sku = sku })));
            }

            if (product.CanSetUnavailable())
            {
                result.Actions.Add(new Action("UnavailableForSale", _urlHelper.Action("UnavailableForSale", "UnavailableForSale", new {sku = sku})));
            }

            return Ok(result);
        }
    }

    public class ProductDto
    {
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public bool ForSale { get; set; }
        public bool FreeShipping { get; set; }

        public IList<Link> Links { get; } = new List<Link>();
        public IList<Action> Actions { get; } = new List<Action>();
    }

    public record Link(string Rel, string Href);
    public record Action(string Name, string Href);

}