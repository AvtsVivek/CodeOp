using System;

namespace EventSourcing.Demo
{
    public interface IEvent
    {
        string EventType { get; }
    }

    public record ProductShipped(string Sku, int Quantity, DateTime DateTime) : IEvent
    {
        public string EventType { get; } = "ProductShipped";
    }

    public record ProductReceived(string Sku, int Quantity, DateTime DateTime) : IEvent
    {
        public string EventType { get; } = "ProductReceived";
    }

    public record InventoryAdjusted(string Sku, int Quantity, string Reason, DateTime DateTime) : IEvent
    {
        public string EventType { get; } = "InventoryAdjusted";
    }
}