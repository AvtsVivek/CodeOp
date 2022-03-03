using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Sales.Products
{
    public class UnavailableForSaleController : ControllerBase
    {
        private readonly SalesDbContext _db;
        private readonly IUrlHelper _urlHelper;

        public UnavailableForSaleController(SalesDbContext db, IUrlHelper urlHelper)
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

            if (product.CanSetUnavailable() == false)
            {
                return BadRequest("Cannot set Unavailable.");
            }

            product.ForSale = false;
            await _db.SaveChangesAsync();

            return Created(_urlHelper.Action("GetSalesProduct", "GetSalesProduct", new { sku }), null);
        }
    }
}