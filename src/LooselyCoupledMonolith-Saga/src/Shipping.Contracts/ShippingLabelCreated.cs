using System;
using NServiceBus;

namespace Shipping.Contracts
{
    public class ShippingLabelCreated : IEvent
    {
        public Guid OrderId { get; set; }
    }
}