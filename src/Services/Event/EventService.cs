using System;
using System.Linq;
using Microsoft.Extensions.Options;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Config;
using verint_service.HttpClients;
using verint_service.Models;

namespace verint_service.Services.Event
{
    public class EventService : IEventService
    {
        private readonly EventTypeConfiguration _eventTypeConfiguration;
        private readonly IHttpClientWrapper _httpClientWrapper;

        public EventService(IOptions<EventTypeConfiguration> eventTypeConfiguration, IHttpClientWrapper httpClientWrapper)
        {
            _eventTypeConfiguration = eventTypeConfiguration.Value;
            _httpClientWrapper = httpClientWrapper;
        }

        public void HandleCaseEvent(CaseEventModel model)
        {
            if (model == null)
            {
                return;
            }

            switch (model.EventType)
            {
                case EventCaseType.PopulatedCloseCaseEvent:
                    HandlePopulatedCloseCaseEvent(model.EventCase);
                    break;
                case EventCaseType.ReclassifyCaseEvent:
                    HandleReclassifyCaseEvent(model.EventCase);
                    break;
            }
        }

        private void HandlePopulatedCloseCaseEvent(EventCase model)
        {
            var selectCaseEvent = _eventTypeConfiguration.PopulatedCloseCaseEvent.FirstOrDefault(_ =>
                string.Equals(_.Type, model.Classification.Type, StringComparison.OrdinalIgnoreCase));

            if (selectCaseEvent == null)
            {
                return;
            }

            _httpClientWrapper.SetHttpClientSecurityHeader(selectCaseEvent.AuthToken);
            _httpClientWrapper.PostAsync(selectCaseEvent.Endpoint, model);
        }

        private void HandleReclassifyCaseEvent(EventCase model)
        {
            var selectCaseEvent = _eventTypeConfiguration.ReclassifyCaseEvent.FirstOrDefault(_ =>
                string.Equals(_.Type, model.Classification.Type, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(_.Subject,  model.Classification.Subject, StringComparison.OrdinalIgnoreCase));

            if (selectCaseEvent == null)
            {
                return;
            }

            _httpClientWrapper.SetHttpClientSecurityHeader(selectCaseEvent.AuthToken);
            _httpClientWrapper.PostAsync(selectCaseEvent.Endpoint, model);
        }
    }
}
