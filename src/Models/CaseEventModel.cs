using StockportGovUK.NetStandard.Models.Verint;

namespace verint_service.Models
{
    public class CaseEventModel
    {
        public EventCaseType EventType { get; set; } = EventCaseType.None;

        public EventCase EventCase { get; set; }
    }

    public enum EventCaseType
    {
        None,

        PopulatedCloseCaseEvent,
        
        ReclassifyCaseEvent
    }
}
