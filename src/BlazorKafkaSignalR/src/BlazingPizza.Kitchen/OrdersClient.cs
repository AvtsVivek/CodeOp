using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazingPizza.Kitchen
{
    public class OrdersClient
    {
        private readonly HttpClient httpClient;

        public OrdersClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<OrderWithStatus>> GetOrders() =>
            await httpClient.GetFromJsonAsync<IEnumerable<OrderWithStatus>>("kitchen/orders");


        public async Task<OrderWithStatus> GetOrder(int orderId) =>
            await httpClient.GetFromJsonAsync<OrderWithStatus>($"kitchen/orders/{orderId}");

        public async Task Prepare(int orderId) => await httpClient.PostAsync($"kitchen/orders/{orderId}/prepare", new StringContent(string.Empty));

        public async Task OutForDelivery(int orderId) => await httpClient.PostAsync($"kitchen/orders/{orderId}/outForDelivery", new StringContent(string.Empty));

        public async Task Deliver(int orderId) => await httpClient.PostAsync($"kitchen/orders/{orderId}/deliver", new StringContent(string.Empty));

        public async Task SubscribeToNotifications(NotificationSubscription subscription)
        {
            var response = await httpClient.PutAsJsonAsync("notifications/subscribe", subscription);
            response.EnsureSuccessStatusCode();
        }
    }
}
