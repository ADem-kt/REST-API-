using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using REST_API_для_симтемы_управления_конфигурациями.Interfaces;
using SignalRApp;
using System;
using System.Data;
using System.Security.Claims;
using REST_API_для_симтемы_управления_конфигурациями.Methods;

namespace REST_API_для_симтемы_управления_конфигурациями
{
    public class Program
    {
        //ISendNotify? SendNotify;
        //public Program(ISendNotify? SendNotify) => this.SendNotify = SendNotify;

        //    ServiceCollection services = (ServiceCollection)new ServiceCollection()
        //.AddTransient<ISendNotify, REST_API_для_симтемы_управления_конфигурациями.Methods.SendNotify>();


        public static void Main(string[] args)
        {
            var adminRole = new Role("admin");
            var userRole = new Role("user");
            var people = new List<Person>
{
    new Person("tom@gmail.com", "12345", adminRole),
    new Person("bob@gmail.com", "55555", userRole),
                };
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");

            builder.Services.AddAuthorization();
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
            app.UseAuthentication();   // добавление middleware аутентификации 
            app.UseAuthorization();   // добавление middleware авторизации 
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
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
            app.MapGet("/login", async context =>
    await SendHtmlAsync(context, "html/login.html"));

            app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
            {
                // получаем из формы email и пароль
                var form = context.Request.Form;
                // если email и/или пароль не установлены, посылаем статусный код ошибки 400
                if (!form.ContainsKey("email") || !form.ContainsKey("password"))
                    return Results.BadRequest("Email и/или пароль не установлены");
                string email = form["email"];
                string password = form["password"];

                // находим пользователя 
                Person? person = people.FirstOrDefault(p => p.Email == email && p.Password == password);
                // если пользователь не найден, отправляем статусный код 401
                if (person is null) return Results.Unauthorized();

                var claims = new List<Claim>
    {
        new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
        new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role.Name)
    };
                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await context.SignInAsync(claimsPrincipal);
                return Results.Redirect(returnUrl ?? "/");
            });
            app.MapGet("/", [Authorize] async (HttpContext context) =>
    await SendHtmlAsync(context, "html/index.html"));

            app.MapGet("/admin", [Authorize(Roles = "admin")] async (HttpContext context) =>
                await SendHtmlAsync(context, "html/admin.html"));


            app.MapGet("/logout", async (HttpContext context) =>
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/login");
            });
            //app.MapPost("/create", async (string mes, IHubContext<ChatHub> hubContext) =>
            //{
            //    try
            //    {
            //        await hubContext.Clients.All.SendAsync("Receive", $"{mes}");
            //        //return $"Добавили строку в конфиг: date = {DateTime.Now}, version = {version}, config_name = {configName}, key = {key}, value = {value}";

            //    }
            //    catch (Exception ex)
            //    {
            //        await hubContext.Clients.All.SendAsync("Receive", $"ERROR: {ex} - {DateTime.Now.ToLongTimeString()}");
            //        //return ex.ToString();
            //    }
            //});
            app.MapHub<ChatHub>("/chat");   // ChatHub будет обрабатывать запросы по пути /chat
            app.MapControllers();
            app.Run();


        }
        static async Task SendHtmlAsync(HttpContext context, string path)
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync(path);
        }
        record class Person(string Email, string Password, Role Role);
        record class Role(string Name);
        public static bool authorized(string email, string password) {
            var adminRole = new Role("admin");
            var userRole = new Role("user");
            var people = new List<Person>
{
    new Person("tom@gmail.com", "12345", adminRole),
    new Person("bob@gmail.com", "55555", userRole),
};          // находим пользователя 
            Person? person = people.FirstOrDefault(p => p.Email == email && p.Password == password);
            // если пользователь не найден, отправляем статусный код 401
            if (person is null) return false;
            else return true;
        }



    }
}
