using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Cohesion
{
    public class GetProductBySkuHandlerDelegateExample : IRequestHandler<GetProductBySkuRequest, ProductViewModel>
    {
        private readonly ProductDelegates.GetProductForSaleBySku _getProduct;

        public GetProductBySkuHandlerDelegateExample(ProductDelegates.GetProductForSaleBySku getProduct)
        {
            _getProduct = getProduct;
        }

        public async Task<ProductViewModel> Handle(GetProductBySkuRequest request, CancellationToken cancellationToken)
        {
            var product = await _getProduct(request.Sku);
            return product.ToViewModel();
        }
    }
}