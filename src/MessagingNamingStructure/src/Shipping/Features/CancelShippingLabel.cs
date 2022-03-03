using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Sales.Contracts;

namespace Shipping.Features
{
    public static class CancelShippingLabel
    {
        public class Command : ICommand
        {
            public Guid OrderId { get; set; }
        }

        public class Handler : IHandleMessages<Command>
        {
            private readonly ShippingDbContext _dbContext;

            public Handler(ShippingDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task Handle(Command message, IMessageHandlerContext context)
            {
                var order = await _dbContext.ShippingLabels.SingleAsync(x => x.OrderId == message.OrderId);
                order.Cancelled = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public class OnOrderCancelled : IHandleMessages<OrderCancelled>
        {
            public async Task Handle(OrderCancelled message, IMessageHandlerContext context)
            {
                await context.SendLocal(new Command
                {
                    OrderId = message.OrderId
                });
            }
        }
    }
}