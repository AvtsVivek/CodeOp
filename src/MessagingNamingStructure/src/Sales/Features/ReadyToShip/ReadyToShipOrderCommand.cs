using System;
using NServiceBus;

namespace Sales.Features
{
    public class ReadyToShipOrderCommand: ICommand
    {
        public Guid OrderId { get; set; }
    }
}