using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus;

namespace Sales.Features
{
    public class ReadyToShipOrderHandler : IHandleMessages<ReadyToShipOrderCommand>
    {
        private readonly SalesDbContext _dbContext;

        public ReadyToShipOrderHandler(SalesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(ReadyToShipOrderCommand message, IMessageHandlerContext context)
        {
            var order = await _dbContext.Orders.SingleAsync(x => x.OrderId == message.OrderId);
            order.Status = OrderStatus.ReadyToShip;
            await _dbContext.SaveChangesAsync();
        }
    }
}