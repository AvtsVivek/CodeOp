using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Sales.Contracts;

namespace Sales.Features
{
    public static class CancelOrder
    {
        public class Command : ICommand
        {
            public Guid OrderId { get; set; }
        }

        public class Handler : IHandleMessages<Command>
        {
            private readonly SalesDbContext _dbContext;

            public Handler(SalesDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task Handle(Command message, IMessageHandlerContext context)
            {
                var order = await _dbContext.Orders.SingleAsync(x => x.OrderId == message.OrderId);
                order.Status = OrderStatus.Cancelled;
                await _dbContext.SaveChangesAsync();

                await context.Publish<OrderCancelled>(cancelled =>
                {
                    cancelled.OrderId = message.OrderId;
                });
            }
        }
    }
}