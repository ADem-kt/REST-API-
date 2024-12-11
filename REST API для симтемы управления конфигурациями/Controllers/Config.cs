using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SignalRApp;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace REST_API_для_симтемы_управления_конфигурациями.Controllers
{
    [Table("Config")]
    [ApiController]
    [Route("[controller]")]
    public class Config : ControllerBase
    {
        [HttpGet(Name = "GetConfig")]
        public string Get(string? s,DateTime? dateTime)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var Configs = db.Configs.ToList();
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (ConfigEF u in Configs)
                    {
                        if (u.config_name.ToString().Contains(s??"")&&u.date>=(dateTime??DateTime.Parse("01.01.1900")))
                        stringBuilder.AppendLine($"{u.row_id} {u.date} {u.version} {u.config_name} {u.key} {u.value}");
                    }
                    return stringBuilder.ToString();
                }
            }
            catch (Exception ex) { return ex.ToString(); }
            
            
        }
    }
}
