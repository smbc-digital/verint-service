using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Helpers
{
    public interface IAssociatedObjectResolver
    {
        FWTObjectBriefDetails Resolve(Case crmCase);
    }
}