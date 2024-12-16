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
                    if (u.user_name.ToString().Contains(s ?? "") && u.date >= (dateTime ?? DateTime.Parse("01.01.1900")))
                        stringBuilder.AppendLine($"{u.row_id} {u.date} {u.version} {u.user_name} {u.key} {u.value}");
                }
            }
            // Assert
            Assert.True(stringBuilder != null);
        }
    }
}