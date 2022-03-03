using System;
using System.Threading.Tasks;
using Billing.Contracts;
using NServiceBus;
using Sales.Contracts;
using Shipping.Contracts;

namespace Shipping
{
    public class CreateShippingLabelHandler : IHandleMessages<CreateShippingLabel>
    {
        public async Task Handle(CreateShippingLabel message, IMessageHandlerContext context)
        {
            // Do some work

            // Reply
            await context.Reply(new ShippingLabelCreated
            {
                OrderId = message.OrderId
            });
        }
    }
}