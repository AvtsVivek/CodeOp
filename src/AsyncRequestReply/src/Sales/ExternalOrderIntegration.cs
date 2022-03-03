using System;
using System.Net.Http;
using System.Threading.Tasks;
using NServiceBus;

namespace Sales
{
    public class ExternalOrderIntegration
    {
        private readonly IMessageSession _messageSession;
        private readonly HttpClient _httpClient;

        public ExternalOrderIntegration(IMessageSession messageSession, HttpClient httpClient)
        {
            _messageSession = messageSession;
            _httpClient = httpClient;
        }

        public async Task PlaceOrder()
        {
            var response = await _httpClient.GetAsync("http://external.com/newOrders");
            var externalOrder = await response.Deserialize<ExternalOrder>();

            var placeOrder = new PlaceOrder
            {
                OrderId = externalOrder!.Id,
                // Translation to convert an external order or PlaceOrder command
            };
            await _messageSession.Send(placeOrder);
        }
    }

    public class ExternalOrder
    {
        public Guid Id { get; set; }
    }
}