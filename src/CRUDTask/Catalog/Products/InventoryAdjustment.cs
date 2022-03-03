using System;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Threading.Tasks;
using Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Warehouse.Products
{
    [ApiController]
    public class InventoryAdjustmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryAdjustmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SwaggerOperation(
            OperationId = "InventoryAdjustment",
            Tags = new[] { "Warehouse" })]
        [HttpPost("/warehouse/products/{sku}/inventoryAdjustment", Name = "InventoryAdjustment")]
        public async Task<IActionResult> InventoryAdjustment([FromRoute] string sku, [FromBody] InventoryAdjustmentRequest request)
        {
            request.Sku = sku;
            await _mediator.Send(request);

            return NoContent();
        }
    }

    public class InventoryAdjustmentRequest : IRequest
    {
        public string Sku { get; set; }
        public int AdjustmentQuantity { get; set; }
    }

    public class InventoryAdjustmentHandler : IRequestHandler<InventoryAdjustmentRequest>
    {
        private readonly CatalogDbContext _db;
        private readonly IMediator _mediator;

        public InventoryAdjustmentHandler(CatalogDbContext db, IMediator mediator)
        {
            _db = db;
            _mediator = mediator;
            _db.Database.EnsureCreated();
        }

        public async Task<Unit> Handle(InventoryAdjustmentRequest request, CancellationToken cancellationToken)
        {
            var product = await _db.Products.SingleOrDefaultAsync(x => x.Sku == request.Sku);
            if (product == null)
            {
                throw new InvalidOperationException();
            }

            product.InventoryAdjustment(request.AdjustmentQuantity);
            await _db.SaveChangesAsync();

            return Unit.Value;
        }
    }

    public class InventoryAdjusted : INotification
    {
        public string Sku { get; }
        public int QuantityOnHand { get; }

        public InventoryAdjusted(string sku, int quantityOnHand)
        {
            Sku = sku;
            QuantityOnHand = quantityOnHand;
        }
    }
}