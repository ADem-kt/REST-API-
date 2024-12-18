using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SignalRApp;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;

namespace REST_API_для_симтемы_управления_конфигурациями.Controllers
{
    [Table("Config")]
    [ApiController]
    [Route("[controller]")]
    public class ApiConfig : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hub;

        public ApiConfig(IHubContext<ChatHub> hub)
        {
            _hub = hub;
        }
        //[HttpGet]
        //public async Task Get(string user, string message)
        //{
        //    await _hub.Clients.All.SendAsync("ReceiveMessage", user, message);
        //}
        [HttpPost(Name = "PostConfig")]
        public async Task<string> PostConfig(string userName, string configName, int version,  string key, string value)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {

                    ConfigEF config1 = new ConfigEF { date = DateTime.Now, user_name = userName, config_name = configName, version = version, key = key, value = value };
                    db.Configs.Add(config1);
                    db.SaveChanges();
                    await _hub.Clients.All.SendAsync("Receive", $"Добавили строку в конфиг: {JsonSerializer.Serialize(config1)}");
                    return $"Добавили строку в конфиг: {JsonSerializer.Serialize(config1)}";
                }
            }
            catch (Exception ex)
            {
                await _hub.Clients.All.SendAsync("Receive", $"ERROR: {ex} - {DateTime.Now.ToLongTimeString()}");
                return ex.ToString();
            }

        }
        [HttpGet(Name = "GetConfig")]
        public IActionResult GetConfig(string? config_name, DateTime? dateTime, bool fromDateTimeToUp=true)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var configs = db.Configs.ToList();
                    StringBuilder mes = new StringBuilder();
                    foreach (ConfigEF config in configs)
                    {
                        if (config.config_name.ToString().Contains(config_name ?? "") && 
                            ((config.date >= (dateTime ?? DateTime.Parse("01.01.1900")) && fromDateTimeToUp)|| (config.date <= (dateTime ?? DateTime.Now) && !fromDateTimeToUp)))
                            mes.AppendLine($"{JsonSerializer.Serialize(config)}");
                    }
                    if (!string.IsNullOrEmpty(mes.ToString()))
                        return StatusCode(200, mes.ToString());
                    else
                    {
                        return StatusCode(400, "По заданным параметрам конфигураций не найдено. Прошу изменить параментры и повторить запрос");
                    }
                }
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
        [HttpPut(Name = "PutConfig")]
        public async Task<IActionResult> PutConfig(string configName, int version, string key, string value) {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var config = db.Configs.ToList().Where(e => e.config_name == configName&&e.version== version&&e.key==key).FirstOrDefault();
                    if (config != null)
                    {
                        config.value = value;
                        db.Configs.Update(config);
                        db.SaveChanges();
                        string mes = $"Изменили строку{JsonSerializer.Serialize(config)}";
                        await _hub.Clients.All.SendAsync("Receive", mes);
                        return StatusCode(200, mes);
                    }
                    else { string mes = $"По параметарм:configName={configName}, version={version}, key={key} конфигураций не найдено. Прошу изменить параментры и повторить запрос"; return StatusCode(400, mes); }
                }
            }
            catch (Exception ex)
            {
                await _hub.Clients.All.SendAsync("Receive", $"ERROR: {ex} - {DateTime.Now.ToLongTimeString()}");
                return StatusCode(500, ex);
            }
        }


    }
}
