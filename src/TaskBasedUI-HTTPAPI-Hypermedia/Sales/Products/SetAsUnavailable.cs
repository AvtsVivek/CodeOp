using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Products;

namespace Sales.Products
{
    public class SetAsUnavailable : INotificationHandler<InventoryAdjusted>
    {
        private readonly SalesDbContext _db;

        public SetAsUnavailable(SalesDbContext db)
        {
            _db = db;
        }

        public async Task Handle(InventoryAdjusted notification, CancellationToken cancellationToken)
        {
            var product = await _db.Products.SingleAsync(x => x.Sku == notification.Sku, cancellationToken);
            product.QuantityOnHand = notification.QuantityOnHand;

            if (product.QuantityOnHand <= 0)
            {
                product.ForSale = false;
            }

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}