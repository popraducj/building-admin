using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BuildingAdmin.Mvc.Models.Https;
using NLog.Web;

namespace BuildingAdmin.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager.LoadConfiguration("Configs/nlog.config").GetCurrentClassLogger();
            try
            {
                BuildWebHost(args).Run(); 
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
        
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>{
                    var env = context.HostingEnvironment;
                    var logger = NLog.LogManager.LoadConfiguration("Configs/nlog.config").GetCurrentClassLogger();
                    var appSettings = $"Configs/appsettings.{env.EnvironmentName}.json";
                    if(!env.EnvironmentName.Equals("Development")){
                        appSettings =$"{AppContext.BaseDirectory}/{appSettings}";
                    }
                    
                    builder.AddJsonFile("Configs/appsettings.json", optional: true, reloadOnChange: true)
                           .AddJsonFile(appSettings, optional: false, reloadOnChange: true);
                    
                })
                .ConfigureLogging(logging =>
                {
                   // logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog()
                .UseUrls("http://localhost:5000")
                .UseStartup<Startup>()
                .UseKestrel(options =>{
                    options.AddServerHeader = false;
                })
                .Build();
    }
}
