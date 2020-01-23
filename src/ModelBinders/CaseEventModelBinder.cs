using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Models;

namespace verint_service.ModelBinders
{
    public class CaseEventModelBinder : RequiredAttribute, IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext context)
        {
            var logger = (ILogger<CaseEventModelBinder>)context.HttpContext.RequestServices.GetService(typeof(ILogger<CaseEventModelBinder>));

            var request = context.HttpContext.Request.Body;

            using (var requestReader = new StreamReader(request))
            {
                var body = requestReader.ReadToEnd();

                XDocument xDocument;
                try
                {
                    xDocument = XDocument.Parse(body);
                }
                catch (Exception ex)
                {
                    logger.LogInformation("Unable to parse request body", ex.InnerException);
                    context.Result = ModelBindingResult.Success(null);
                    return Task.CompletedTask;
                }

                var eventCaseTypeSuccess = Enum.TryParse(xDocument.Root?.Name.LocalName, out EventCaseType caseEventType);
                if (!eventCaseTypeSuccess)
                {
                    logger.LogInformation($"EventType: {xDocument.Root?.Name.LocalName} not configured");
                    logger.LogInformation($"EventBody: {body}");
                    context.Result = ModelBindingResult.Success(null);
                    return Task.CompletedTask;
                }

                var serializedCase = xDocument.Root?.FirstNode?.ToString();
                if (serializedCase == null)
                {
                    logger.LogInformation("Unable to serialize case from xml response");
                    context.Result = ModelBindingResult.Success(null);
                    return Task.CompletedTask;
                }

                var serializer = new XmlSerializer(typeof(EventCase));
                using (var caseReader = new StringReader(serializedCase))
                {
                    EventCase deserializedCase;
                    try
                    {
                        deserializedCase = (EventCase)serializer.Deserialize(caseReader);
                    }
                    catch (Exception ex)
                    {
                        logger.LogInformation("Unable to parse serialized case into EventCase", ex.InnerException);
                        context.Result = ModelBindingResult.Success(null);
                        return Task.CompletedTask;
                    }

                    context.Result = ModelBindingResult.Success(new CaseEventModel{
                        EventType = caseEventType,
                        EventCase = deserializedCase
                    });
                }
            }

            return Task.CompletedTask;
        }
    }
}
