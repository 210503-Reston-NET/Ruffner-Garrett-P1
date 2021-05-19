using Service;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using Data.Entities;
using System.Net.Mail;
using System.Net;

namespace UI
{
    public class MenuFactory
    {

        public static IMenu GetMenu(string menuType)
        {
            
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("App/appsettings.json")
            .Build();

            var smtpClient = new SmtpClient(configuration["Smtp:Host"])
            {
                Port = int.Parse(configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(configuration["Smtp:Username"], configuration["Smtp:Password"]),
                EnableSsl = true,
            };

            string connectionString = configuration.GetConnectionString("StoreDB");

            DbContextOptions<p0Context> options = new DbContextOptionsBuilder<p0Context>()
            .UseSqlServer(connectionString)
            .Options;
            
            var context = new p0Context(options);

            switch(menuType.ToLower()){
                case "mainmenu":
                    return new MainMenu(new Services(new RepoDB(context), new EmailService(smtpClient)), new ValidationUI());
                case "adminmenu":
                    return new AdminMenu(new Services(new RepoDB(context), new EmailService(smtpClient)), new ValidationUI());
                case "inventorymenu":
                    return new InventoryMenu(new Services(new RepoDB(context), new EmailService(smtpClient)), new ValidationUI());
                default:
                    return null;
            }
        }
    }
}