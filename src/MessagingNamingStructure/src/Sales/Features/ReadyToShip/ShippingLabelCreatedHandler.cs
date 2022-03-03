using System.Threading.Tasks;
using NServiceBus;
using Shipping.Contracts;

namespace Sales.Features
{
    public class ShippingLabelCreatedHandler : IHandleMessages<ShippingLabelCreatedEvent>
    {
        public async Task Handle(ShippingLabelCreatedEvent message, IMessageHandlerContext context)
        {
            await context.SendLocal(new ReadyToShipOrderCommand
            {
                OrderId = message.OrderId
            });
        }
    }
}