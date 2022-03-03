using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cohesion
{
    public interface IProductService
    {
        Task UpdateProductInfo();
        Task<Product> GetProductBySku(string sku);
        Task<Product> GetProductForSaleBySku(string sku);
        Task<List<Product>> GetProducts();
    }
}