using System;
using System.IO;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
namespace azure_play
{
    class Program
    {
        static void Main(string[] args)
        {
            // Log.Logger = new LoggerConfiguration()
            //     .MinimumLevel.Debug()
            //     .WriteTo.Console()
            //     .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
            //     .CreateLogger();

            // Log.Information("Hello, world!");

            // int a = 10, b = 0;
            // try
            // {
            //     Log.Debug("Dividing {A} by {B}", a, b);
            //     Console.WriteLine(a / b);
            // }
            // catch (Exception ex)
            // {
            //     Log.Error(ex, "Something went wrong");
            // }

            // Log.CloseAndFlush();

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Hello, world!");
            
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services)=>{
                    services.AddTransient<TestService>();
                })
                .UseSerilog()
                .Build();

            var svc = ActivatorUtilities.CreateInstance<TestService>(host.Services);
            svc.Run();

        }

        static void BuildConfig(IConfigurationBuilder builder){
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables();
        }
    }

    public class TestService
    {   
        private readonly ILogger<TestService> _log;
        private readonly IConfiguration _config;
        public TestService(ILogger<TestService> log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public void Run(){
            int a = 10, b = 0;
            try
            {
                _log.LogDebug("Dividing {A} by {B}", a, b);
                Console.WriteLine(a / b);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Something went wrong");
            }

            int t = _config.GetValue<int>("test_num");
            _log.LogInformation($"test num:  {t} \n");
            
            var valuesSection = _config.GetSection("Services");

            foreach (IConfigurationSection section in valuesSection.GetChildren())
            {
                var key = section.GetValue<string>("name");
                var value = section.GetValue<string>("id");

                _log.LogDebug($"Service Name:  {key}  id: {value} ");

            }
        }
    }
}
