using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using SignalRApp;
using System;

namespace REST_API_для_симтемы_управления_конфигурациями
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();      // подключема сервисы SignalR.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            //app.MapPost("/create", async (int version, string configName, string key, string value,  IHubContext<ChatHub> hubContext) =>
            //{
            //    try
            //    {
            //        using (ApplicationContext db = new ApplicationContext())
            //        {
            //            ConfigEF config1 = new ConfigEF { date = DateTime.Now, version = version, user_name = configName, key = key, value = value };
            //            db.Configs.Add(config1);
            //            db.SaveChanges();
            //            await hubContext.Clients.All.SendAsync("Receive", $"Добавили строку в конфиг: date = {DateTime.Now}, version = {version}, config_name = {configName}, key = {key}, value = {value}");
            //            return $"Добавили строку в конфиг: date = {DateTime.Now}, version = {version}, config_name = {configName}, key = {key}, value = {value}";
            //        }
            //    }
            //    catch (Exception ex) { 
            //        await hubContext.Clients.All.SendAsync("Receive", $"ERROR: {ex} - {DateTime.Now.ToLongTimeString()}");  
            //        return ex.ToString(); }
            //});
            app.MapHub<ChatHub>("/chat");   // ChatHub будет обрабатывать запросы по пути /chat
            app.MapGet("/", () => "Hello World!");
            app.MapControllers();
            app.Run();
            
    }

    }
}
