using verint_service.Models;
using VerintWebService;

namespace verint_service.Builders
{
    public interface ICaseFormBuilder
    {
        FWTCaseForm Build(Case crmCase);
    }
}