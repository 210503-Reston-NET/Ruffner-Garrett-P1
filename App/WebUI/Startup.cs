using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Data;
using Service;
using System.Net.Mail;
using System.Net;

namespace WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var smtpClient = new SmtpClient(Configuration["Smtp:Host"])
            {
                Port = int.Parse(Configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(Configuration["Smtp:Username"], Configuration["Smtp:Password"]),
                EnableSsl = true,
            };
            services.AddControllersWithViews();
            services.AddDbContext<StoreDBContext>(options =>options.UseNpgsql(parseElephantSQLURL(this.Configuration.GetConnectionString("StoreDB"))));
            services.AddScoped<IRepository, RepoDB>();
            //services.AddScoped<IEmailService, EmailService>(options => options.);
            services.AddSingleton<IEmailService>( new EmailService(smtpClient));
            services.AddScoped<IServices, Services>();
            
            
        }
        public static string parseElephantSQLURL(string uriString)
        {
            //var uriString = connectionString; //ConfigurationManager.AppSettings["ELEPHANTSQL_URL"] ?? 'postgres://localhost/mydb;
            var uri = new Uri(uriString);
            var db = uri.AbsolutePath.Trim('/');
            var user = uri.UserInfo.Split(':')[0];
            var passwd = uri.UserInfo.Split(':')[1];
            var port = uri.Port > 0 ? uri.Port : 5432;
            var connStr = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}",
                uri.Host, db, user, passwd, port);
            return connStr;
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
