using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Cohesion
{
    public class GetProductBySkuRequest : IRequest<ProductViewModel>
    {
        public GetProductBySkuRequest(string sku)
        {
            Sku = sku;
        }

        public string Sku { get; set; }
    }

    public class GetProductBySkuHandler : IRequestHandler<GetProductBySkuRequest, ProductViewModel>
    {
        private readonly IProductService _productService;

        public GetProductBySkuHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<ProductViewModel> Handle(GetProductBySkuRequest request, CancellationToken cancellationToken)
        {
            var product = await _productService.GetProductForSaleBySku(request.Sku);
            return product.ToViewModel();
        }
    }
}