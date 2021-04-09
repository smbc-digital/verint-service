using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using verint_service.Attributes;
using verint_service.Helpers;
using verint_service.Helpers.VerintConnection;
using verint_service.Models.Config;
using verint_service.Services;
using verint_service.Services.Case;
using verint_service.Services.Event;
using verint_service.Services.Individual;
using verint_service.Services.Individual.Weighting;
using verint_service.Services.Organisation;
using verint_service.Services.Organisation.Weighting;
using verint_service.Services.Property;
using verint_service.Services.Street;
using verint_service.Services.Update;
using verint_service.Services.VerintOnlineForm;
using verint_service.Utils.Builders;
using verint_service.Utils.Mappers;

namespace verint_service.Utils.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<VerintConnectionConfiguration>(configuration.GetSection("VerintConnectionConfiguration"));
            services.Configure<EventTypeConfiguration>(configuration.GetSection("EventTypeConfiguration"));
        }

        public static void RegisterHelpers(this IServiceCollection services)
        {
            services.AddSingleton<IVerintConnection, VerintConnection>();
            services.AddTransient<IClientMessageInspector, RequestInspector>();
            services.AddTransient<IEndpointBehavior, RequestBehavior>();
            services.AddTransient<IAssociatedObjectResolver, AssociatedObjectResolver>();
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<ICaseService, CaseService>();
            services.AddTransient<IUpdateService, UpdateService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IIndividualService, IndividualService>();
            services.AddTransient<IInteractionService, InteractionService>();
            services.AddTransient<IPropertyService, PropertyService>();
            services.AddTransient<IStreetService, StreetService>();
            services.AddTransient<IOrganisationService, OrganisationService>();
            services.AddTransient<IVerintOnlineFormService, VerintOnlineFormService>();
        }

        public static void RegisterUtils(this IServiceCollection services)
        {
            services.AddSingleton<IIndividualWeighting, verint_service.Services.Individual.Weighting.EmailWeighting>();
            services.AddSingleton<IIndividualWeighting, verint_service.Services.Individual.Weighting.DateOfBirthWeighting>();
            services.AddSingleton<IIndividualWeighting, verint_service.Services.Individual.Weighting.TelephoneWeighting>();
            services.AddSingleton<IIndividualWeighting, verint_service.Services.Individual.Weighting.AlternativeTelephoneWeighting>();
            services.AddSingleton<IIndividualWeighting, verint_service.Services.Individual.Weighting.UprnWeighting>();
            services.AddSingleton<IIndividualWeighting, verint_service.Services.Individual.Weighting.AddressWeighting>();

            services.AddSingleton<IOrganisationWeighting, verint_service.Services.Organisation.Weighting.NameWeighting>();
            services.AddSingleton<IOrganisationWeighting, verint_service.Services.Organisation.Weighting.TelephoneWeighting>();
            services.AddSingleton<IOrganisationWeighting, verint_service.Services.Organisation.Weighting.EmailWeighting>();
            services.AddSingleton<IOrganisationWeighting, verint_service.Services.Organisation.Weighting.UprnWeighting>();
            services.AddSingleton<IOrganisationWeighting, verint_service.Services.Organisation.Weighting.AddressWeighting>();
            
            services.AddTransient<ICaseFormBuilder, CaseFormBuilder>();
            services.AddSingleton<CaseToFWTCaseCreateMapper>();
            services.AddSingleton<ICaseFormBuilder, CaseFormBuilder>();
        }

        public static void RegisterAttributes(this IServiceCollection services)
        {
            services.AddSingleton(new DevelopmentOnlyAttribute(services.BuildServiceProvider().GetService<IWebHostEnvironment>()));
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Verint service API", Version = "v1" });
                c.IncludeXmlComments($"./swagger-documentation.xml");
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "Authorization using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
                c.CustomSchemaIds(x => x.FullName);
            });
        }

        public static void AddStorageProvider(this IServiceCollection services, IConfiguration configuration)
        {
            var storageProviderConfiguration = configuration.GetSection("StorageProvider");            
            
            switch (storageProviderConfiguration["Type"])
            {
                case "Redis":
                    services.AddStackExchangeRedisCache(options => 
                    {
                        options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                        {
                            EndPoints = 
                            {
                                { storageProviderConfiguration["Address"] ?? "127.0.0.1",  6379}
                            },
                            ClientName = storageProviderConfiguration["Name"] ?? Assembly.GetEntryAssembly()?.GetName().Name,
                            SyncTimeout = 30000,
                            AsyncTimeout = 30000
                        };
                    });
                    break;
                case "None":
                    break;
                default:
                    services.AddDistributedMemoryCache();
                    break;
            }
        }
    }
}
