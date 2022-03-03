using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BlazingPizza.Server
{
    public class KitchenOrderHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}