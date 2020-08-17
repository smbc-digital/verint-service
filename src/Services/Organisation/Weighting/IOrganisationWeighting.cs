using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Services.Organisation.Weighting
{
    public interface IOrganisationWeighting
    {
        int Calculate(FWTOrganisation organisationObject, StockportGovUK.NetStandard.Models.Verint.Organisation organisation);
    }
}