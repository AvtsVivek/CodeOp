using System;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.BuyerAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using NServiceBus;

namespace Microsoft.eShopWeb.Features.PlaceOrder
{
    public class EmailOrderConfirmation : IHandleMessages<OrderPlacedEvent>
    {
        private readonly IEmailSender _emailSender;

        public EmailOrderConfirmation(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task Handle(OrderPlacedEvent message, IMessageHandlerContext context)
        {
            await _emailSender.SendEmailAsync("test@test.com", $"Order Confirmation", "Thank you for your order.");
        }
    }
}