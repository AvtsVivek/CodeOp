using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using NServiceBus;
using NServiceBus.Encryption.MessageProperty;

namespace Microsoft.eShopWeb.Features.PlaceOrder
{
    public class PlaceOrderCommand : ICommand
    {
        public string BuyerId { get; set; }
        public Address Address { get; set; }
        public List<OrderItem> Items { get; set; }
        public CreditCardEncrypted CreditCard { get; set; }

        public class CreditCardEncrypted
        {
            public EncryptedString Number { get; set; }
            public EncryptedString Expiry { get; set; }
            public EncryptedString Cvv { get; set; }
        }
    }

    public class PlaceOrderHandler : IHandleMessages<PlaceOrderCommand>
    {
        private readonly IPaymentGateway _paymentGateway;

        public PlaceOrderHandler(IPaymentGateway paymentGateway)
        {
            _paymentGateway = paymentGateway;
        }

        public async Task Handle(PlaceOrderCommand message, IMessageHandlerContext context)
        {
            await _paymentGateway.ProcessCreditCard(message.CreditCard.Number.Value, message.CreditCard.Expiry.Value, message.CreditCard.Cvv.Value);
        }
    }
}













