using Microsoft.Extensions.Configuration;
using Serilog;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace App_WPF_007
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Application Starting");

            // Register global exception handlers
            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler.HandleException;
            DispatcherUnhandledException += GlobalExceptionHandler.HandleDispatcherException;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Application Exiting");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
