using System.Linq;

namespace EventSourcing.Demo
{
    public class Projection
    {
        private readonly ProductDbContext _dbContext;

        public Projection(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ReceiveEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case ProductShipped shipProduct:
                    Apply(shipProduct);
                    break;
                case ProductReceived receiveProduct:
                    Apply(receiveProduct);
                    break;
            }
        }

        public Product GetProduct(string sku)
        {
            var product = _dbContext.Products.SingleOrDefault(x => x.Sku == sku);
            if (product == null)
            {
                product = new Product
                {
                    Sku = sku
                };
                _dbContext.Products.Add(product);
            }

            return product;
        }

        private void Apply(ProductShipped shipProduct)
        {
            var product = GetProduct(shipProduct.Sku);
            product.Shipped += shipProduct.Quantity;
            _dbContext.SaveChanges();
        }

        private void Apply(ProductReceived productReceived)
        {
            var state = GetProduct(productReceived.Sku);
            state.Received += productReceived.Quantity;
            _dbContext.SaveChanges();
        }
    }
}