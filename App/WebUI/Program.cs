using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
           
            // var builder = new ConfigurationBuilder()
            // .SetBasePath(Environment.CurrentDirectory)
            // .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            // // .AddJsonFile($"appsettings.json", optional: true)
            // .AddEnvironmentVariables();
            // Configuration = builder.Build();
            // Log.Logger = new LoggerConfiguration()
            //     .MinimumLevel.Verbose()
            //     .WriteTo.File("App/logs/app.txt", rollingInterval: RollingInterval.Day)
            //     .CreateLogger();
            // Log.Logger = new LoggerConfiguration()
            //     .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
            //     .Enrich.FromLogContext()
            //     .WriteTo.File("../logs/app.txt",rollingInterval: RollingInterval.Hour)
            //     .CreateLogger();

            //Logging Config
            // var builder = new ConfigurationBuilder()
            // .SetBasePath(env.ContentRootPath)
            // .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            // .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            // .AddEnvironmentVariables();
            // this.Configuration = builder.Build();

            // Log.Logger = new LoggerConfiguration()
            // .ReadFrom.Configuration(Configuration)
            // .CreateLogger();

            Log.Logger.Information("Logging Configuration Loaded");

            try
            {
                CreateHostBuilder(args).Build().Run();
                Log.Information("Started web host");
                
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return;
            }
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)             
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
