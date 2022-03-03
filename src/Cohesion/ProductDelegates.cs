using System.Threading.Tasks;

namespace Cohesion
{
    public static class ProductDelegates
    {
        public delegate Task<Product> GetProductBySku(string sku);
        public delegate Task<Product> GetProductForSaleBySku(string sku);
    }
}