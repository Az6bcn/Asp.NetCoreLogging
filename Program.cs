using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.IO;

namespace Logging
{
    public class Program
    {
        /* https://github.com/serilog/serilog-aspnetcore
         * https://github.com/serilog/serilog-settings-configuration : Configuration is read from the Serilog section in appsettings.json
         * https://github.com/serilog/serilog-sinks-file
         */

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                                                                .AddEnvironmentVariables()
                                                                .Build();

        public static void Main(string[] args)
        {
            // configure serilog to read from the configuration file (appsettings.json)
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .WriteTo.File(new JsonFormatter(), path: @"c:\temp\logs\loggingTesting.json", shared: true)
            .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /**
         *  CreateDefaultBuilder() adds the following logging providers by default: Console, Debug, EventSource and EventLog(only Windows)
         *  A logging provider displays or stores logs.
         *  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.0
         **/
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
    }
}
