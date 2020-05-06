using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using verint_service.Helpers;
using verint_service.Helpers.VerintConnection;
using verint_service.Models.Config;
using verint_service.Services;
using verint_service.Services.Case;
using verint_service.Services.Event;
using verint_service.Services.Organisation;
using verint_service.Services.Property;
using verint_service.Services.Street;
using verint_service.Services.Update;
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
        }

        public static void RegisterUtils(this IServiceCollection services)
        {
            services.AddSingleton<IIndividualWeighting, EmailWeighting>();
            services.AddSingleton<IIndividualWeighting, DateOfBirthWeighting>();
            services.AddSingleton<IIndividualWeighting, NameWeighting>();
            services.AddSingleton<IIndividualWeighting, TelephoneWeighting>();
            services.AddSingleton<IIndividualWeighting, AlternativeTelephoneWeighting>();
            services.AddSingleton<IIndividualWeighting, UprnWeighting>();
            services.AddSingleton<IIndividualWeighting, AddressWeighting>();
            services.AddTransient<ICaseFormBuilder, CaseFormBuilder>();
            services.AddSingleton<CaseToFWTCaseCreateMapper>();
            services.AddSingleton<ICaseFormBuilder, CaseFormBuilder>();
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Verint service API", Version = "v1" });
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
    }
}
