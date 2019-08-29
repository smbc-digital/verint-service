using System.Collections.Generic;

namespace verint_service.Config
{
    public class EventTypeConfiguration
    {
        public List<CaseEventConfiguration> PopulatedCloseCaseEvent { get; set; }
        public List<CaseEventConfiguration> ReclassifyCaseEvent { get; set; }
    }

    public class CaseEventConfiguration
    {
        public string Subject { get; set; }
        public string Type { get; set; }
        public string Endpoint { get; set; }
        public string AuthToken { get; set; }
    }
}
