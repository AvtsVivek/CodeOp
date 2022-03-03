using System.Threading.Tasks;
using Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Sales.Products
{
    public class UnavailableForSaleController : ControllerBase
    {
        private readonly CatalogDbContext _db;
        private readonly IUrlHelper _urlHelper;

        public UnavailableForSaleController(CatalogDbContext db, IUrlHelper urlHelper)
        {
            _db = db;
            _urlHelper = urlHelper;
        }

        [SwaggerOperation(
            OperationId = "UnavailableForSale",
            Tags = new[] { "Sales" })]
        [HttpPost("/sales/products/{sku}/unavailableForSale")]
        public async Task<IActionResult> UnavailableForSale([FromRoute] string sku)
        {
            var product = await _db.Products.SingleOrDefaultAsync(x => x.Sku == sku);
            if (product == null)
            {
                return NotFound();
            }

            product.UnavailableForSale();

            await _db.SaveChangesAsync();

            return Created(_urlHelper.Action("GetSalesProduct", "GetSalesProduct", new { sku }), null);
        }
    }
}