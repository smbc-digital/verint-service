using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockportGovUK.AspNetCore.Middleware;
using StockportGovUK.AspNetCore.Availability;
using StockportGovUK.AspNetCore.Availability.Middleware;
using StockportGovUK.AspNetCore.Gateways;
using StockportGovUK.AspNetCore.Gateways.InthubGateway;
using StockportGovUK.AspNetCore.Polly;
using Swashbuckle.AspNetCore.Swagger;
using verint_service.Helpers.VerintConnection;
using verint_service.Models.Config;
using verint_service.Services.Case;
using verint_service.Services.Event;
using verint_service.Services.Update;

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
            services.Configure<VerintConnectionConfiguration>(Configuration.GetSection("VerintConnectionConfiguration"));
            services.Configure<EventCaseConfiguration>(Configuration.GetSection("EventCaseConfiguration"));

            services.AddTransient<ICaseService, CaseService>();
            services.AddTransient<IUpdateService, UpdateService>();
            services.AddSingleton<IVerintConnection, VerintConnection>();
            services.AddTransient<IClientMessageInspector, RequestInspector>();
            services.AddTransient<IEndpointBehavior, RequestBehavior>();
            services.AddTransient<IEventService, EventService>();

            services.AddSingleton<IInthubGateway, InthubGateway>();
            services.AddHttpClients<IGateway, Gateway>(Configuration);

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "verint_service API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "Authorization using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                });
            });

            services.AddHttpClient();

            services.AddAvailability();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            
            app.UseMiddleware<Availability>();
            app.UseMiddleware<ExceptionHandling>();
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "verint_service API");
            });
            app.UseMvc();
        }
    }
}
