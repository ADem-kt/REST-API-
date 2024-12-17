using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalRApp
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task Send(string message, string userName)
        {
            await Clients.All.SendAsync("Receive", message, userName);
        }
        [Authorize(Roles = "admin")]
        public async Task Notify(string message)
        {
            await Clients.All.SendAsync("Receive", message, "Администратор");
        }
    }
    //[Authorize]
    //public async Task Send(string message, string userName)
    //{
    //    await Clients.All.SendAsync("Receive", message, userName);
    //}
    //[Authorize(Roles = "admin")]
    //public async Task Notify(string message)
    //{
    //    await Clients.All.SendAsync("Receive", message, "Администратор");
    //}
    //public override async Task OnConnectedAsync()
    //{
    //    await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} вошел в чат");
    //    await base.OnConnectedAsync();
    //}

    //public override async Task OnDisconnectedAsync(Exception? exception)
    //{
    //    await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} покинул в чат");
    //    await base.OnDisconnectedAsync(exception);
    //}
//}
}