using System.Threading;
using System.Threading.Tasks;
using Catalog;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Products;

namespace Sales.Products
{
    public class SetAsUnavailable : INotificationHandler<InventoryAdjusted>
    {
        private readonly CatalogDbContext _db;

        public SetAsUnavailable(CatalogDbContext db)
        {
            _db = db;
        }

        public async Task Handle(InventoryAdjusted notification, CancellationToken cancellationToken)
        {
            var product = await _db.Products.SingleAsync(x => x.Sku == notification.Sku, cancellationToken);
            product.InventoryAdjustment(notification.QuantityOnHand);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}