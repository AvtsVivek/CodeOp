using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazingPizza.Events;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Server.Kitchen
{
    [Route("kitchen/orders")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly PizzaStoreContext _db;

        public OrdersController(PizzaStoreContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
        {
            var orders = await _db.Orders
                .Include(o => o.DeliveryLocation)
                .Include(o => o.Pizzas).ThenInclude(p => p.Special)
                .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
                .OrderByDescending(o => o.CreatedTime)
                .ToListAsync();

            return orders.Select(o => OrderWithStatus.FromOrder(o)).ToList();
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderWithStatus>> GetOrderWithStatus(int orderId)
        {
            var order = await _db.Orders
                .Where(o => o.OrderId == orderId)
                .Include(o => o.DeliveryLocation)
                .Include(o => o.Pizzas).ThenInclude(p => p.Special)
                .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
                .SingleOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return OrderWithStatus.FromOrder(order);
        }

        [HttpPost("{orderId}/prepare")]
        public async Task<ActionResult<OrderWithStatus>> Preparing(int orderId, [FromServices] ICapPublisher capPublisher)
        {
            var order = await _db.Orders
                .Where(o => o.OrderId == orderId)
                .SingleOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = OrderStatus.Preparing;
            await _db.SaveChangesAsync();

            await capPublisher.PublishAsync(nameof(OrderBeingPreparedEvent), new OrderBeingPreparedEvent(orderId));

            return NoContent();
        }

        [HttpPost("{orderId}/outForDelivery")]
        public async Task<ActionResult<OrderWithStatus>> OutForDelivery(int orderId, [FromServices] ICapPublisher capPublisher)
        {
            var order = await _db.Orders
                .Where(o => o.OrderId == orderId)
                .SingleOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = OrderStatus.OutForDelivery;
            await _db.SaveChangesAsync();

            await capPublisher.PublishAsync(nameof(OrderBeingPreparedEvent), new OrderBeingPreparedEvent(orderId));

            return NoContent();
        }

        [HttpPost("{orderId}/deliver")]
        public async Task<ActionResult<OrderWithStatus>> Deliver(int orderId, [FromServices] ICapPublisher capPublisher)
        {
            var order = await _db.Orders
                .Where(o => o.OrderId == orderId)
                .SingleOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = OrderStatus.Delivered;
            await _db.SaveChangesAsync();

            await capPublisher.PublishAsync(nameof(OrderDeliveredEvent), new OrderDeliveredEvent(orderId));

            return NoContent();
        }
    }
}
