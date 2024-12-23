using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using REST_API_config_control.EF;
using SignalRApp;
using System;
using System.Text;
using System.Text.Json;

namespace REST_API_config_control.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribeController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hub;

        public SubscribeController(IHubContext<ChatHub> hub)
        {
            _hub = hub;
        }
        [HttpPost(Name = "PostSubscribe")]
        public async Task<IActionResult> PostSubscribe(string login, string password, int subscribeTypeKey)
        {
            try
            {
                string mes;
                StringBuilder sb = new StringBuilder();
                if (!Program.authorized(login, password))
                    return StatusCode(401, "Неверный Логин/Пароль");
                if (!SubscribeType.subscibeTypeDictionary.TryGetValue(subscribeTypeKey, out string subscribeTypeValue))
                {
                    mes = $"{subscribeTypeKey} - Неизвестный тип подписки.";
                    return StatusCode(400, mes.ToString());
                }
                using (ApplicationContext db = new ApplicationContext())
                {
                    SubscribeEF subscribe = new() { date = DateTime.Now, login = login, password = password, subscribeTypeKey = subscribeTypeKey };
                    db.Subscribes.Add(subscribe);
                    db.SaveChanges();
                    sb.AppendLine($"Добавили подписку на событие {subscribeTypeValue} : {JsonSerializer.Serialize(subscribe)}");
                    sb.AppendLine("Присылаю полный список конфигураций:");

                    var configs = db.Configs.ToList();
                    if (configs.Count == 0)
                    {
                        sb.AppendLine("Список конфиураций пуст");
                        return StatusCode(200, sb.ToString());
                    }
                    foreach (ConfigEF config in configs)
                    {
                        sb.AppendLine($"{JsonSerializer.Serialize(config)}");
                    }
                    await _hub.Clients.All.SendAsync("Receive", sb.ToString());
                    return StatusCode(200, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
        [HttpDelete(Name = "DeleteSubscribe")]
        public async Task<IActionResult> DeleteSubscribe(string login, string password, int subscribeTypeKey)
        {
            try
            {
                string mes;
                if (!Program.authorized(login, password))
                    return StatusCode(401, "Неверный Логин/Пароль");
                if (!SubscribeType.subscibeTypeDictionary.TryGetValue(subscribeTypeKey, out string subscribeTypeValue))
                {
                    mes = $"{subscribeTypeKey} - Неизвестный тип подписки.";
                    return StatusCode(400, mes.ToString());
                }
                using (ApplicationContext db = new ApplicationContext())
                {
                    var subscribe = db.Subscribes.ToList().Where(e => e.login == login && e.password == password && e.subscribeTypeKey == subscribeTypeKey).FirstOrDefault();
                    if (subscribe == null) {
                        mes = $"Подписка еще не создана";
                        return StatusCode(400, mes.ToString());
                    }

                    db.Subscribes.Remove(subscribe);
                    db.SaveChanges();

                    mes = $"Подписка отменена {subscribeTypeValue} : {JsonSerializer.Serialize(subscribe)}";
                    await _hub.Clients.All.SendAsync("Receive", mes);
                }
                return StatusCode(200, mes.ToString());

            }
            catch (Exception ex)
            {
                await _hub.Clients.All.SendAsync("Receive", $"ERROR: {ex} - {DateTime.Now.ToLongTimeString()}");
                return StatusCode(500, ex.ToString());
            }

        }
    }
}
