using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Utils.Builders
{
    public interface ICaseFormBuilder
    {
        FWTCaseForm Build(Case crmCase);
    }
}