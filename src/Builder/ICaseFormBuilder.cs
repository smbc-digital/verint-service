using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Builders
{
    public interface ICaseFormBuilder
    {
        FWTCaseForm Build(Case crmCase);
    }
}