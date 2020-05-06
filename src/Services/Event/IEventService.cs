using verint_service.Models;
using verint_service.Models.CaseEvent;

namespace verint_service.Services.Event
{
    public interface IEventService
    {
        void HandleCaseEvent(CaseEventModel model);
    }
}
