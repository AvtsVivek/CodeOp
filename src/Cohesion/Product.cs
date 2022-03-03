using System.Threading.Tasks;

namespace Cohesion
{
    public class Product
    {
        public string Sku { get; set; }
    }

    public class ProductViewModel
    {
        public string Sku { get; set; }
    }

    public static class Extensions
    {
        public static ProductViewModel ToViewModel(this Product product)
        {
            return new()
            {
                Sku = product.Sku
            };
        }
    }
}