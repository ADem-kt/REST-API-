using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using REST_API_для_симтемы_управления_конфигурациями.Controllers;
using System;
using System.Text;

namespace REST_API_для_симтемы_управления_конфигурациями.Tests
{
    public class configTests
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            ApplicationContext db = new ApplicationContext();
            db.Database.ExecuteSqlRawAsync("delete TABLE [Configs]");
            // Act
            ConfigEF config1 = new ConfigEF { date = DateTime.Now, version = 1, config_name = "configName1", key = "key", value = "value" };
            db.Configs.Add(config1);
            var res = db.SaveChanges();
            db.Remove(config1);
            // Assert
            Assert.True(res != 0);
        }
        [Fact]
        public void Test2()
        {
            // Arrange
            string? s = null;
            DateTime? dateTime = null;
            StringBuilder stringBuilder = new StringBuilder();
            using (ApplicationContext db = new ApplicationContext())
            // Act
            {
                var Configs = db.Configs.ToList();
                foreach (ConfigEF u in Configs)
                {
                    if (u.config_name.ToString().Contains(s ?? "") && u.date >= (dateTime ?? DateTime.Parse("01.01.1900")))
                        stringBuilder.AppendLine($"{u.row_id} {u.date} {u.version} {u.config_name} {u.key} {u.value}");
                }
            }
            // Assert
            Assert.True(stringBuilder != null);
        }
    }
}