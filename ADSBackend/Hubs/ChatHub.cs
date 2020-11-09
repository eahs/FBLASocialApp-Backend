using ADSBackend.Models;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ADSBackend.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var httpUser = Context.User;

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public void SendGroupMessage(string chatPrivateKey, string message)
        {
            ChatMessage msg = new ChatMessage
            {
                Body = message
            };

            Clients.Group(chatPrivateKey).SendAsync("Receive", msg);
        }

        public async Task Subscribe(string chatPrivateKey)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatPrivateKey);

            //await Clients.Group(chatPrivateKey).SendAsync("Receive", $"{Context.ConnectionId} has joined the group {chatPrivateKey}.");
        }

        public async Task Unsubscribe(string chatPrivateKey)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatPrivateKey);

            //await Clients.Group(chatPrivateKey).SendAsync("Receive", $"{Context.ConnectionId} has left the group {chatPrivateKey}.");
        }
    }
}
