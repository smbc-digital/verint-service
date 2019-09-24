using verint_service.Models;
using VerintWebService;

namespace verint_service.Helpers
{
    public interface IAssociatedObjectHelper
    {
        FWTObjectBriefDetails GetAssociatedObject(Case crmCase);
    }
}