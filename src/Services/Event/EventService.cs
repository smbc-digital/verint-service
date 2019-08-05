using StockportGovUK.AspNetCore.Gateways.InthubGateway;
using verint_service.Models;

namespace verint_service.Services.Event
{
    public class EventService : IEventService
    {
        private readonly IInthubGateway _inthubGateway;

        public EventService(IInthubGateway inthubGateway)
        {
            _inthubGateway = inthubGateway;
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
            }
        }

        private void HandlePopulatedCloseCaseEvent(EventCase model)
        {
            switch (model.Classification.Subject.ToLower())
            {
                case "fostering":
                    _inthubGateway.UnmatchFosteringCase(model.Id);
                    break;
            }
        }
    }
}
