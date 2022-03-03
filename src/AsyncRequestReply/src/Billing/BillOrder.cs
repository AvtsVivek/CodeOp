using System.Threading.Tasks;
using Billing.Contracts;
using NServiceBus;
using Sales.Contracts;

namespace Billing
{
    public class BillOrderHandler : IHandleMessages<BillOrder>
    {
        public async Task Handle(BillOrder message, IMessageHandlerContext context)
        {
            // Do some work

            // Reply
            await context.Reply(new OrderBilled
            {
                OrderId = message.OrderId
            });
        }
    }
}