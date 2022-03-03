using System;
using NServiceBus;

namespace Shipping.Contracts
{
    public class ShippingLabelCreated : IMessage
    {
        public Guid OrderId { get; set; }
    }
}