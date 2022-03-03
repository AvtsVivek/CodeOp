using System;
using NServiceBus;

namespace Billing.Contracts
{
    public class OrderBilled : IMessage
    {
        public Guid OrderId { get; set; }
    }
}