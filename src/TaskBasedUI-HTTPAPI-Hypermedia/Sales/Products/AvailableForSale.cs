using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Sales.Products
{
    public class AvailableForSaleController : ControllerBase
    {
        private readonly SalesDbContext _db;
        private readonly IUrlHelper _urlHelper;

        public AvailableForSaleController(SalesDbContext db, IUrlHelper urlHelper)
        {
            _db = db;
            _urlHelper = urlHelper;
        }

        [SwaggerOperation(
            OperationId = "AvailableForSale",
            Tags = new[] { "Sales" })]
        [HttpPost("/sales/products/{sku}/availableForSale")]
        public async Task<IActionResult> AvailableForSale([FromRoute] string sku)
        {
            var product = await _db.Products.SingleOrDefaultAsync(x => x.Sku == sku);
            if (product == null)
            {
                return NotFound();
            }

            if (product.CanSetAvailable() == false)
            {
                return BadRequest("Cannot set available.");
            }

            product.ForSale = true;
            await _db.SaveChangesAsync();

            return Created(_urlHelper.Action("GetSalesProduct", "GetSalesProduct", new { sku }), null);
        }
    }
}