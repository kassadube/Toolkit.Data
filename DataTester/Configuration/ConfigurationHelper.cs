using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;


    public static class ConfigurationHelper
    {
        public static IConfiguration LoadAppConfiguration()
        {         
            IConfiguration config = null;
            try
            {
                config = new ConfigurationBuilder()                    
                    .AddJsonFile($"appsettings.json")
                    .AddJsonFile($"appsettings.production.json", true, true)

                    .Build();
            }
            catch (System.Exception)
            {
            }
            return config;
        }
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                // loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddSerilog(dispose: true);
            });
        }

        public static void ConfigLog(IConfiguration _config)
        {
            var logDir = _config["Serilog:LogDirectory"];           
            string outputTemplate = _config["Serilog:outputTemplate"] ?? "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level}] [{Message}] [{Exception}]{NewLine}";
            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(_config)
                    .Enrich.FromLogContext()
                    .WriteTo.RollingFile(Path.Combine(logDir, "log-{Date}.txt"), outputTemplate: outputTemplate)
                    .CreateLogger();

            Console.WriteLine("INIT LOGGER");
        }
    }

