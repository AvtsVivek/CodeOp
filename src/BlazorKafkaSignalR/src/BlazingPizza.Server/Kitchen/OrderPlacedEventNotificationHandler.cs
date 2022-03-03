using System.Threading.Tasks;
using BlazingPizza.Events;
using DotNetCore.CAP;
using Microsoft.AspNetCore.SignalR;

namespace BlazingPizza.Server
{
    public class OrderPlacedEventNotificationHandler : ICapSubscribe
    {
        private readonly IHubContext<KitchenOrderHub> _hubContext;

        public OrderPlacedEventNotificationHandler(IHubContext<KitchenOrderHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [CapSubscribe("orders", Group = nameof(OrderPlacedEventNotificationHandler))]
        public async Task Handle(OrderPlacedEvent orderPlacedEvent)
        {
            await _hubContext.Clients.All.SendAsync("OrderPlaced");
        }
    }
}