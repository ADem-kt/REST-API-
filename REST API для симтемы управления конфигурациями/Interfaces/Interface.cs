namespace REST_API_для_симтемы_управления_конфигурациями.Interfaces
{
    public interface ISendNotify
    {
        public Task OnSendConfigChangeNotifyToSubscribedUsers(string method,int SubscibeTypeKey, string message);
        
    }
}
