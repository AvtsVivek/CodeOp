using System;
using NServiceBus;

namespace Shipping.Contracts
{
    public class ShippingLabelCreatedEvent : IEvent
    {
        public Guid OrderId { get; set; }
    }
}