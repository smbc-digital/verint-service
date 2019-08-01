using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using verint_service.Models;

namespace verint_service.ModelBinders
{
    public class CaseEventModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext context)
        {
            var logger = (ILogger<CaseEventModelBinder>)context.HttpContext.RequestServices.GetService(typeof(ILogger<CaseEventModelBinder>));

            var request = context.HttpContext.Request.Body;

            using (var requestReader = new StreamReader(request))
            {
                var body = requestReader.ReadToEnd();
                logger.LogWarning($"**DEBUG: {body}");

                XDocument xDocument;
                try
                {
                    xDocument = XDocument.Parse(body);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to parse request body", ex.InnerException);
                }

                var eventCaseTypeSuccess = Enum.TryParse(xDocument.Root?.Name.LocalName, out EventCaseType caseEventType);
                if (!eventCaseTypeSuccess)
                {
                    throw new Exception("EventType not configured");
                }

                var serializedCase = xDocument.Root?.FirstNode?.ToString();
                if (serializedCase == null)
                {
                    throw new Exception("Unable to serialize case from xml response");
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
                        throw new Exception("Unable to parse serialized case into EventCase", ex.InnerException);
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
