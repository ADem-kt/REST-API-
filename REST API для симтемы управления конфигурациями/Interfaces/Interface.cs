namespace REST_API_config_control.Interfaces
{
    public interface ISendNotify
    {
        public Task OnSendConfigChangeNotifyToSubscribedUsers(string method,int SubscibeTypeKey, string message);
        
    }
}
