using verint_service.Models;

namespace verint_service.Services.Event
{
    public interface IEventService
    {
        void HandleCaseEvent(CaseEventModel model);
    }
}
