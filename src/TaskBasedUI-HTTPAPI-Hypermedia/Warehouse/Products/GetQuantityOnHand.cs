using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Warehouse.Products
{
    [ApiController]
    public class GetQuantityOnHandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetQuantityOnHandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SwaggerOperation(
            OperationId = "GetQuantityOnHand",
            Tags = new[] { "Warehouse" })]
        [HttpGet("/warehouse/products/{sku}", Name = "GetQuantityOnHand")]
        public async Task<IActionResult> GetQuantityOnHand([FromRoute] string sku)
        {
            var result = await _mediator.Send(new GetQuantityOnHandRequest(sku));
            if (result.Exists)
            {
                return Ok(result.Result);
            }

            return NotFound();
        }
    }

    public class GetQuantityOnHandRequest : IRequest<(bool Exists, GetQuantityOnHandResult Result)>
    {
        public GetQuantityOnHandRequest(string sku)
        {
            Sku = sku;
        }

        public string Sku { get; set; }
    }

    public class GetQuantityOnHandResult
    {
        public string Sku { get; set; }
        public int QuantityOnHand { get; set; }


    }

    public record Link(string Rel, string Href);
    public record Action(string Name, string Href);

    public class GetQuantityOnHandHandler : IRequestHandler<GetQuantityOnHandRequest, (bool Exists, GetQuantityOnHandResult Result)>
    {
        private readonly WarehouseDbContext _db;

        public GetQuantityOnHandHandler(WarehouseDbContext db)
        {
            _db = db;
            _db.Database.EnsureCreated();
        }

        public async Task<(bool Exists, GetQuantityOnHandResult Result)> Handle(GetQuantityOnHandRequest request, CancellationToken cancellationToken)
        {
            var product = await _db.Products
                .Where(x => x.Sku == request.Sku)
                .Select(x => new {x.QuantityOnHand})
                .SingleOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return (false, null);
            }

            var result = new GetQuantityOnHandResult
            {
                Sku = request.Sku,
                QuantityOnHand = product.QuantityOnHand
            };

            return (true, result);
        }
    }
}