using NServiceBus;

namespace Microsoft.eShopWeb.Features.PlaceOrder
{
    public class OrderPlacedEvent : IEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
    }
}