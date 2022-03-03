using MediatR;

namespace Purchasing.Products
{
    public class PurchaseOrderRequisition : IRequest
    {
        public PurchaseOrderRequisition(string sku)
        {

        }
    }
}