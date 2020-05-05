using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockportGovUK.AspNetCore.Availability;
using StockportGovUK.AspNetCore.Availability.Middleware;
using StockportGovUK.AspNetCore.Middleware;
using System.Diagnostics.CodeAnalysis;
using verint_service.Builders;
using verint_service.Config;
using verint_service.HttpClients;
using verint_service.Mappers;
using verint_service.Models.Config;
using verint_service.Utils.HealthChecks;
using verint_service.Utils.ServiceCollectionExtensions;

namespace verint_service
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<VerintConnectionConfiguration>(Configuration.GetSection("VerintConnectionConfiguration"));
            services.Configure<EventTypeConfiguration>(Configuration.GetSection("EventTypeConfiguration"));

            services.AddTransient<ICaseFormBuilder, CaseFormBuilder>();
            services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
            services.AddSingleton<CaseToFWTCaseCreateMapper>();
            services.AddSingleton<ICaseFormBuilder, CaseFormBuilder>();
            services.AddSwagger();
            services.AddHttpClient();
            services.AddAvailability();
            services.AddHealthChecks()
                  .AddCheck<TestHealthCheck>("TestHealthCheck");

            services.RegisterHelpers();
            services.RegisterServices();
            services.RegisterUtils();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment("local"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMiddleware<Availability>();
            app.UseMiddleware<ApiExceptionHandling>();
            app.UseHttpsRedirection();

            app.UseHealthChecks("/healthcheck", HealthCheckConfig.Options);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{(env.IsEnvironment("local") ? string.Empty : "/verintservice")}/swagger/v1/swagger.json", "Verint service API");
            });
        }
    }
}
