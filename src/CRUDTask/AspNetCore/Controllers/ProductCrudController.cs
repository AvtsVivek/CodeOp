using System.Threading.Tasks;
using Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Controllers
{
    public class ProductCrudController : Controller
    {
        private readonly CatalogDbContext _catalogDbContext;

        public ProductCrudController(CatalogDbContext catalogDbContext)
        {
            _catalogDbContext = catalogDbContext;
            _catalogDbContext.Database.EnsureCreated();
        }

        [HttpGet("/productCrud/{sku}")]
        public async Task<IActionResult> GetProduct(string sku)
        {
            var record = await _catalogDbContext.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Sku == sku);

            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        [HttpPost("/productCrud/{sku}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] string sku, [FromForm]CatalogProduct product)
        {
            var record = await _catalogDbContext.Products.SingleOrDefaultAsync(x => x.Sku == sku);

            if (product == null)
            {
                return NotFound();
            }

            record.Description = product.Description;
            record.Name = product.Name;
            /*
            record.Price = product.Price;
            record.Cost = product.Cost;
            record.QuantityOnHand = product.QuantityOnHand;
            record.ForSale = product.Price > 0 && product.QuantityOnHand > 0 && product.ForSale;
            record.FreeShipping = product.FreeShipping;
            */

            await _catalogDbContext.SaveChangesAsync();

            return RedirectToAction("GetProduct", new { sku = sku });
        }
    }
}