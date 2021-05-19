using System;
using Serilog;
namespace UI
{
    class UI
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File("App/logs/app.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

        
            //Dont need db context here
            // var configuration = new ConfigurationBuilder()
            // .SetBasePath(Directory.GetCurrentDirectory())
            // .AddJsonFile("App/appsettings.json")
            // .Build();

            // string connectionString = configuration.GetConnectionString("StoreDB");

            // DbContextOptions<p0Context> options = new DbContextOptionsBuilder<p0Context>()
            // .UseSqlServer(connectionString)
            // .Options;
            
            // var context = new p0Context(options);


            

            Log.Verbose("Starting Main Menu");
            try{
            MenuFactory.GetMenu("mainmenu").Start();
            }catch(Exception ex){
                Log.Error(ex.Message);
            }

            Log.CloseAndFlush();
           

        }
    }
}
