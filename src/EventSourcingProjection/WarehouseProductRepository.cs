using System;
using System.Collections.Generic;

namespace EventSourcing.Demo
{
    public class WarehouseProductRepository
    {
        private readonly List<Action<IEvent>> _projectionCallbacks = new();
        private readonly Dictionary<string, List<IEvent>> _inMemoryStreams = new();

        public WarehouseProduct Get(string sku)
        {
            var warehouseProduct = new WarehouseProduct(sku);

            if (_inMemoryStreams.ContainsKey(sku))
            {
                foreach (var evnt in _inMemoryStreams[sku])
                {
                    warehouseProduct.ApplyEvent(evnt);
                }
            }

            return warehouseProduct;
        }

        public void Save(WarehouseProduct warehouseProduct)
        {
            if (_inMemoryStreams.ContainsKey(warehouseProduct.Sku) == false)
            {
                _inMemoryStreams.Add(warehouseProduct.Sku, new List<IEvent>());
            }

            var newEvents = warehouseProduct.GetUncommittedEvents();
            _inMemoryStreams[warehouseProduct.Sku].AddRange(newEvents);
            warehouseProduct.EventsCommitted();

            foreach (var newEvent in newEvents)
            {
                foreach (var callback in _projectionCallbacks)
                {
                    callback(newEvent);
                }
            }
        }

        public void Subscribe(Action<IEvent> callback)
        {
            _projectionCallbacks.Add(callback);
        }
    }
}