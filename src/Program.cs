using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws;

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
            var logConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            Log.Logger = Configurator.Create(Configuration, logConfig);

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(Configuration)
                .UseSerilog()
                .Build();
    }
}
