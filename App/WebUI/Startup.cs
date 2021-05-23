using System.Reflection;
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
using Serilog;
using WebUI.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using StoreModels;

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
            services.AddDbContext<StoreDBContext>(options =>options.UseNpgsql(parseElephantSQLURL(this.Configuration.GetConnectionString("StoreDB"))));
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddScoped<IRepository, RepoDB>();
            services.AddSingleton<IEmailService>( new EmailService(smtpClient));
            services.AddScoped<IServices, Services>();
            services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<StoreDBContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(opts => 
                {
                    opts.User.RequireUniqueEmail = false;
                    opts.SignIn.RequireConfirmedAccount = false;
                }
            );
            // services.AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<StoreDBContext>().AddDefaultTokenProviders();
            // services.AddIdentity<ApplicationUser, UserRole>(cfg => {
            //     cfg.User.RequireUniqueEmail = true;
            // }).AddEntityFrameworkStores<StoreDBContext>();

            // services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider();
            // services.Configure<RazorViewEngineOptions>(o =>
            // {
            //     o.AreaViewLocationFormats.Add("/Areas/{2}/{0}" + RazorViewEngine.ViewExtension);
            // });
            

            Log.Debug("Services Configured");
        }
        public static string parseElephantSQLURL(string uriString)
        {
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
            // var builder = new ConfigurationBuilder()
            // .SetBasePath(env.ContentRootPath)
            // .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            // .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            // .AddEnvironmentVariables();
            // this.Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                
                //  endpoints.MapControllerRoute(
                //     name: "MyArea",
                //     pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
            });
        }
    }
}