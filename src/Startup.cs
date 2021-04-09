using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockportGovUK.AspNetCore.Availability;
using StockportGovUK.AspNetCore.Availability.Middleware;
using StockportGovUK.AspNetCore.Middleware;
using StockportGovUK.NetStandard.Gateways;
using StockportGovUK.NetStandard.Gateways.Extensions;
using verint_service.Utils.Extensions;
using verint_service.Utils.HealthChecks;
using ServiceCollectionExtensions = StockportGovUK.NetStandard.Gateways.Extensions.ServiceCollectionExtensions;
using StockportGovUK.NetStandard.Abstractions.Caching;
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
            services.AddControllers()
                    .AddNewtonsoftJson();

            services.AddHttpClient<IGateway, Gateway>()
                .AddPolicyHandler(ServiceCollectionExtensions.GetWaitAndRetryForeverPolicy())
                .AddPolicyHandler(ServiceCollectionExtensions.GetCircuitBreakerPolicy());

            //services.AddAvailability();
            services.AddSwagger();
            services.AddHealthChecks()
                    .AddCheck<TestHealthCheck>("TestHealthCheck");

            services.RegisterConfiguration(Configuration);
            services.RegisterHelpers();
            services.RegisterServices();
            services.RegisterAttributes();
            services.RegisterUtils();          
            
            services.AddStorageProvider(Configuration);     
            services.Configure<CacheProviderConfiguration>(Configuration.GetSection("CacheProviderConfiguration"));
            services.AddSingleton<ICacheProvider, CacheProvider>();
            
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

            //app.UseMiddleware<Availability>();
            app.UseMiddleware<ApiExceptionHandling>();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseHealthChecks("/healthcheck", HealthCheckConfig.Options);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Verint service API");
            });
        }
    }
}
