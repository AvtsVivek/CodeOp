using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.Infrastructure.Data;

namespace Microsoft.eShopWeb.Features.Orders
{
    [Route("order/my-orders")]
    public class GetMyOrdersController : Controller
    {
        private readonly IMediator _mediator;

        public GetMyOrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var viewModel = await _mediator.Send(new GetMyOrdersQuery(User.Identity.Name));

            return View("/Features/Orders/GetMyOrders.cshtml", viewModel);
        }
    }

    public class GetMyOrdersViewModel
    {
        public IEnumerable<OrderSummaryViewModel> Orders { get; set; }
    }

    public class OrderSummaryViewModel
    {
        private const string DEFAULT_STATUS = "Pending";

        public int OrderNumber { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public decimal Total { get; set; }
        public string Status => DEFAULT_STATUS;
    }

    public class GetMyOrdersQuery : IRequest<GetMyOrdersViewModel>
    {
        public string UserName { get; set; }

        public GetMyOrdersQuery(string userName)
        {
            UserName = userName;
        }
    }

    public class GetMyOrdersHandler : IRequestHandler<GetMyOrdersQuery, GetMyOrdersViewModel>
    {
        private readonly CatalogContext _db;

        public GetMyOrdersHandler(CatalogContext db)
        {
            _db = db;
        }

        public async Task<GetMyOrdersViewModel> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            var result = new GetMyOrdersViewModel();
            result.Orders = await _db.Orders
                .Include(x => x.OrderItems)
                .Where(x => x.BuyerId == request.UserName)
                .Select(o => new OrderSummaryViewModel
                {
                    OrderDate = o.OrderDate,
                    OrderNumber = o.Id,
                    Total = o.OrderItems.Sum(x => x.Units * x.UnitPrice),
                })
                .ToArrayAsync(cancellationToken: cancellationToken);

            return result;
        }
    }
}