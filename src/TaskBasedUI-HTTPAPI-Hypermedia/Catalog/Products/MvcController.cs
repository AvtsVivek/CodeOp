using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Products
{
    public class MvcController : Controller
    {
        private readonly CatalogDbContext _db;

        public MvcController(CatalogDbContext db)
        {
            _db = db;
            _db.Database.EnsureCreated();
        }

        [HttpGet("/mvc/")]
        public async Task<IActionResult> Get([FromQuery]string sku)
        {
            var product = await _db.Products.SingleOrDefaultAsync(x => x.Sku == sku);
            if (product == null)
            {
                return NotFound();
            }

            return View("mvc", product);
        }

        [HttpGet("/mvc/images")]
        public async Task<IActionResult> Images([FromQuery]string sku)
        {
            var images = await _db.ProductImages.Where(x => x.Sku == sku).ToArrayAsync();
            return View("images", images);
        }

        [HttpPost("/mvc/save")]
        public async Task<IActionResult> Save([FromQuery]string sku, [FromForm]ProductResponse productResponse)
        {
            var product = await _db.Products.SingleOrDefaultAsync(x => x.Sku == sku);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = productResponse.Name;
            product.Description = productResponse.Description;
            await _db.SaveChangesAsync();

            return RedirectToAction("Get", new { sku });
        }
    }
}