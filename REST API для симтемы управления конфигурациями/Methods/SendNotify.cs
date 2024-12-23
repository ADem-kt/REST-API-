using Microsoft.AspNetCore.SignalR;
using REST_API_для_симтемы_управления_конфигурациями.EF;
using REST_API_для_симтемы_управления_конфигурациями.Interfaces;
using SignalRApp;
using System.Collections.Generic;
using System.Text.Json;

namespace REST_API_для_симтемы_управления_конфигурациями.Methods
{
     public class SendNotify :ISendNotify
    {
        private readonly IHubContext<ChatHub> _hub;

        public SendNotify(IHubContext<ChatHub> hub)
        {
            _hub = hub;
        }
        public async Task OnSendConfigChangeNotifyToSubscribedUsers(string method,int subscribeTypeKey, string message)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    List<string> list = new List<string>();

                    foreach (UserConnection uc in ChatHub.uList)
                    {
                        var Subscribe = db.Subscribes.Where(e => e.login == uc.UserName).FirstOrDefault();
                        if (Subscribe != null)
                        {
                            switch (Subscribe.subscribeTypeKey)
                            {
                                case 1:
                                    await _hub.Clients.Clients(uc.ConnectionID).SendAsync(method, message);
                                    break;
                                case 2:
                                    if (subscribeTypeKey == Subscribe.subscribeTypeKey)
                                        await _hub.Clients.Clients(uc.ConnectionID).SendAsync(method, message);
                                    break;
                            }
                        };
                }
                    foreach (var Subscribe in db.Subscribes)
                    {
                        switch (Subscribe.subscribeTypeKey)
                        {
                            case 1:

                                Subscribe.savedMesseges = Subscribe.savedMesseges + Environment.NewLine + message;
                                break;
                            case 2:
                                if (subscribeTypeKey == Subscribe.subscribeTypeKey)
                                    Subscribe.savedMesseges = Subscribe.savedMesseges + Environment.NewLine + message;
                                break;
                        }
                    }
                    db.SaveChanges();
                }
            }
            catch { }; 
            
        }
    }
}
