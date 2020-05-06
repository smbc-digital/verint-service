using System;
using System.Linq;
using Microsoft.Extensions.Options;
using StockportGovUK.NetStandard.Gateways;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Models.CaseEvent;
using verint_service.Models.Config;

namespace verint_service.Services.Event
{
    public class EventService : IEventService
    {
        private readonly EventTypeConfiguration _eventTypeConfiguration;
        private readonly IGateway _gateway;

        public EventService(IOptions<EventTypeConfiguration> eventTypeConfiguration, IGateway gateway)
        {
            _eventTypeConfiguration = eventTypeConfiguration.Value;
            _gateway = gateway;
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

            _gateway.ChangeAuthenticationHeader(selectCaseEvent.AuthToken);
            _gateway.PostAsync(selectCaseEvent.Endpoint, model);
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

            _gateway.ChangeAuthenticationHeader(selectCaseEvent.AuthToken);
            _gateway.PostAsync(selectCaseEvent.Endpoint, model);
        }
    }
}
