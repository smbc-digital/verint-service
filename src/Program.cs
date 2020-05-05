using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;

namespace verint_service
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("./Config/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"./Config/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddJsonFile("./Config/Secrets/appsettings.secrets.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"./Config/Secrets/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.secrets.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteToElasticsearchAws(Configuration)
                .CreateLogger();

            WebProxy proxy = new WebProxy("http://172.16.0.126:8080", false);
            WebRequest.DefaultWebProxy = proxy;
            BuildHost(args).Run();
        }

        public static IHost BuildHost(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseStartup<Startup>();
                  webBuilder.UseConfiguration(Configuration);
              })
              .UseSerilog()
              .Build();
    }
}
