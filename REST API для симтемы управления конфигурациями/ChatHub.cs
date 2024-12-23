using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using REST_API_config_control.EF;
using System;
using System.Security.Claims;

namespace SignalRApp
{
    public class UserConnection
    {
        public string UserName { set; get; }
        public string ConnectionID { set; get; }
    }
    [Authorize]
    public class ChatHub : Hub
    {
        static public readonly List<UserConnection> uList = new List<UserConnection>();
        public async Task Send(string message, string userName)
        {
            string datetime = DateTime.Now.ToString();
            await Clients.All.SendAsync("Receive", $"{datetime} {Context.ConnectionId} {message}");
        }
        //[Authorize(Roles = "admin")]
        //public async Task Notify(string message)
        //{
        //    await Clients.All.SendAsync("Receive", message, "Администратор");
        //}
    
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
    public override async Task OnConnectedAsync()
        {
            try
            {
                var us = new UserConnection();//класс содержит все актуальные подключения
                us.UserName = Context.User.Identity.Name;
                us.ConnectionID = Context.ConnectionId;
                uList.Add(us);
                await Clients.All.SendAsync("Notify", $"{DateTime.Now} {us.UserName} {us.ConnectionID} вошел в чат");
                using (ApplicationContext db = new ApplicationContext())
                { 
                    if (!string.IsNullOrWhiteSpace(db.Subscribes.Where(e => e.login == us.UserName).FirstOrDefault().savedMesseges))
                    {
                        string[] masString = db.Subscribes.Where(e => e.login == us.UserName).FirstOrDefault().savedMesseges.Split(Environment.NewLine);
                        foreach (string s in masString)
                            Clients.Caller.SendAsync("Receive", s);
                    }
                }
                await base.OnConnectedAsync();
            }
            catch (Exception ex) {}
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            uList.Remove(uList.Where(e => e.ConnectionID == Context.ConnectionId).FirstOrDefault());
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} покинул в чат");
            await base.OnDisconnectedAsync(exception);
        }
    }
}