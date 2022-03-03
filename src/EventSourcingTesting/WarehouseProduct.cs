using System;
using System.Collections.Generic;

namespace EventSourcing.Demo
{
    public class WarehouseProductState
    {
        public int QuantityOnHand { get; set; }
    }

    public abstract class AggregateRoot
    {
        private readonly IList<IEvent> _uncommittedEvents = new List<IEvent>();

        public IList<IEvent> GetUncommittedEvents()
        {
            return _uncommittedEvents;
        }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        protected void Add(IEvent evnt)
        {
            _uncommittedEvents.Add(evnt);
        }

        public abstract void Load(IEnumerable<IEvent> events);
    }

    public class WarehouseProduct : AggregateRoot
    {
        public string Sku { get; }

        private readonly WarehouseProductState _warehouseProductState = new();

        public WarehouseProduct(string sku)
        {
            Sku = sku;
        }

        public override void Load(IEnumerable<IEvent> events)
        {
            foreach (var evnt in events)
            {
                Apply(evnt as dynamic);
            }
        }

        public static WarehouseProduct Load(string sku, IEnumerable<IEvent> events)
        {
            var warehouseProduct = new WarehouseProduct(sku);
            warehouseProduct.Load(events);
            return warehouseProduct;
        }

        public WarehouseProductState GetState()
        {
            return _warehouseProductState;
        }

        public void ShipProduct(int quantity)
        {
            if (quantity > _warehouseProductState.QuantityOnHand)
            {
                throw new InvalidDomainException("Cannot Ship to a negative Quantity on Hand.");
            }

            var productShipped = new ProductShipped(Sku, quantity, DateTime.UtcNow);

            Apply(productShipped);
            Add(productShipped);
        }

        private void Apply(ProductShipped evnt)
        {
            _warehouseProductState.QuantityOnHand -= evnt.Quantity;
        }

        public void ReceiveProduct(int quantity)
        {
            var productReceived = new ProductReceived(Sku, quantity, DateTime.UtcNow);

            Apply(productReceived);
            Add(productReceived);
        }

        private void Apply(ProductReceived evnt)
        {
            _warehouseProductState.QuantityOnHand += evnt.Quantity;
        }

        public void AdjustInventory(int quantity, string reason)
        {
            if (_warehouseProductState.QuantityOnHand + quantity < 0)
            {
                throw new InvalidDomainException("Cannot adjust to a negative Quantity on Hand.");
            }

            var inventoryAdjusted = new InventoryAdjusted(Sku, quantity, reason, DateTime.UtcNow);

            Apply(inventoryAdjusted);
            Add(inventoryAdjusted);
        }

        private void Apply(InventoryAdjusted evnt)
        {
            _warehouseProductState.QuantityOnHand += evnt.Quantity;
        }

        public int GetQuantityOnHand()
        {
            return _warehouseProductState.QuantityOnHand;
        }

    }

    public class InvalidDomainException : Exception
    {
        public InvalidDomainException(string message) : base(message)
        {

        }
    }
}