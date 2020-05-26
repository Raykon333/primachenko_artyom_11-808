using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GachiMail.Utilities.FileLogger;
using System.IO;

namespace GachiMail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            /*var factory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(
                    new FileLoggerProvider
                    (Path
                    .Combine(Directory
                    .GetCurrentDirectory(), "log.txt")));
                builder.AddConsole();
            });*/
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
