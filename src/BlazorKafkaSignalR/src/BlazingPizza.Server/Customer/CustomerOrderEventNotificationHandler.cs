using System.Threading.Tasks;
using BlazingPizza.Events;
using DotNetCore.CAP;
using Microsoft.AspNetCore.SignalR;

namespace BlazingPizza.Server.Customer
{
    public class CustomerOrderEventNotificationHandler : ICapSubscribe
    {
        private readonly IHubContext<CustomerOrderHub> _hubContext;

        public CustomerOrderEventNotificationHandler(IHubContext<CustomerOrderHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [CapSubscribe(nameof(OrderBeingPreparedEvent), Group = nameof(CustomerOrderEventNotificationHandler) + ":" + nameof(OrderBeingPreparedEvent))]
        public async Task Handle(OrderBeingPreparedEvent orderBeingPreparedEvent)
        {
            await _hubContext.Clients.Group(orderBeingPreparedEvent.OrderId.ToString()).SendAsync("OrderBeingPrepared");
        }

        [CapSubscribe(nameof(OrderPickedUpForDeliveryEvent), Group = nameof(CustomerOrderEventNotificationHandler) + ":" + nameof(OrderPickedUpForDeliveryEvent))]
        public async Task Handle(OrderPickedUpForDeliveryEvent orderPickedUpForDeliveryEvent)
        {
            await _hubContext.Clients.Group(orderPickedUpForDeliveryEvent.OrderId.ToString()).SendAsync("OrderPickedUpForDelivery");
        }

        [CapSubscribe(nameof(OrderDeliveredEvent), Group = nameof(CustomerOrderEventNotificationHandler) + ":" + nameof(OrderDeliveredEvent))]
        public async Task Handle(OrderDeliveredEvent orderDeliveredEvent)
        {
            await _hubContext.Clients.Group(orderDeliveredEvent.OrderId.ToString()).SendAsync("OrderDelivered");
        }
    }
}