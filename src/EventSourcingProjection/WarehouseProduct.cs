using System;
using System.Collections.Generic;

namespace EventSourcing.Demo
{
    public class CurrentState
    {
        public int QuantityOnHand { get; set; }
    }

    public class WarehouseProduct
    {
        public string Sku { get; }
        private readonly IList<IEvent> _allEvents = new List<IEvent>();
        private readonly IList<IEvent> _uncommittedEvents = new List<IEvent>();

        // Projection (Current State)
        private readonly CurrentState _currentState = new();

        public WarehouseProduct(string sku)
        {
            Sku = sku;
        }

        public void ShipProduct(int quantity)
        {
            if (quantity > _currentState.QuantityOnHand)
            {
                throw new InvalidDomainException("Ah... we don't have enough product to ship?");
            }

            AddEvent(new ProductShipped(Sku, quantity, DateTime.UtcNow));
        }

        public void ReceiveProduct(int quantity)
        {
            AddEvent(new ProductReceived(Sku, quantity, DateTime.UtcNow));
        }

        public void AdjustInventory(int quantity, string reason)
        {
            if (_currentState.QuantityOnHand + quantity < 0)
            {
                throw new InvalidDomainException("Cannot adjust to a negative quantity on hand.");
            }

            AddEvent(new InventoryAdjusted(Sku, quantity, reason, DateTime.UtcNow));
        }

        private void Apply(ProductShipped evnt)
        {
            _currentState.QuantityOnHand -= evnt.Quantity;
        }

        private void Apply(ProductReceived evnt)
        {
            _currentState.QuantityOnHand += evnt.Quantity;
        }

        private void Apply(InventoryAdjusted evnt)
        {
            _currentState.QuantityOnHand += evnt.Quantity;
        }

        public void ApplyEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case ProductShipped shipProduct:
                    Apply(shipProduct);
                    break;
                case ProductReceived receiveProduct:
                    Apply(receiveProduct);
                    break;
                case InventoryAdjusted inventoryAdjusted:
                    Apply(inventoryAdjusted);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported Event.");
            }

            _allEvents.Add(evnt);
        }

        public void AddEvent(IEvent evnt)
        {
            ApplyEvent(evnt);
            _uncommittedEvents.Add(evnt);
        }

        public IList<IEvent> GetUncommittedEvents()
        {
            return new List<IEvent>(_uncommittedEvents);
        }

        public IList<IEvent> GetAllEvents()
        {
            return new List<IEvent>(_allEvents);
        }

        public void EventsCommitted()
        {
            _uncommittedEvents.Clear();
        }

        public int GetQuantityOnHand()
        {
            return _currentState.QuantityOnHand;
        }
    }

    public class InvalidDomainException : Exception
    {
        public InvalidDomainException(string message) : base(message)
        {

        }
    }
}