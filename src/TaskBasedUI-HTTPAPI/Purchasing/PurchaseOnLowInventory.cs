using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Purchasing.Products;
using Warehouse.Products;

namespace Purchasing
{
    public class PurchaseOnLowInventory : INotificationHandler<InventoryAdjusted>
    {
        private const int LowInventoryThreshold = 5;
        private readonly IMediator _mediator;

        public PurchaseOnLowInventory(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(InventoryAdjusted notification, CancellationToken cancellationToken)
        {
            if (notification.QuantityOnHand >= LowInventoryThreshold)
            {
                return;
            }

            await _mediator.Send(new PurchaseOrderRequisition(notification.Sku));
        }
    }
}