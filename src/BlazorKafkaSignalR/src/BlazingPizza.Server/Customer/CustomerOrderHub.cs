using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Server.Customer
{
    public class CustomerOrderHub : Hub
    {
        private readonly PizzaStoreContext _db;

        public CustomerOrderHub(PizzaStoreContext db)
        {
            _db = db;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        private string GetUserId()
        {
            return Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task WatchOrder(int orderId)
        {
            var userId = GetUserId();
            var exists = await _db.Orders.AnyAsync(x => x.UserId == userId && x.OrderId == orderId);
            if (exists)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, orderId.ToString());
            }
        }
    }
}