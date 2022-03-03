using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using NServiceBus;

namespace Microsoft.eShopWeb.Features.PlaceOrder
{
    public class PlaceOrderCommand : ICommand
    {
        public string BuyerId { get; set; }
        public Address Address { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class PlaceOrderHandler : IHandleMessages<PlaceOrderCommand>
    {
        private readonly IAsyncRepository<Order> _orderRepository;

        public PlaceOrderHandler(IAsyncRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(PlaceOrderCommand message, IMessageHandlerContext context)
        {
            var order = new Order(message.BuyerId, message.Address, message.Items);
            await _orderRepository.AddAsync(order);

            await context.Publish(new OrderPlacedEvent
            {
                BuyerId = message.BuyerId,
                OrderId = order.Id
            });
        }
    }
}