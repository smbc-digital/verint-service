using System.Collections.Generic;
using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Verint;
using StockportGovUK.NetStandard.Models.Verint.Lookup;
using VerintWebService;

namespace verint_service.Services.Organisation
{
    public interface IOrganisationService
    {
        Task<StockportGovUK.NetStandard.Models.Verint.Organisation> GetAsync(string id);
        Task<FWTObjectID> CreateAsync(StockportGovUK.NetStandard.Models.Verint.Organisation organisation);

        Task<IEnumerable<OrganisationSearchResult>> SearchByNameAsync(string organisationName);

        Task<FWTObjectID> MatchAsync(StockportGovUK.NetStandard.Models.Verint.Organisation organisation);

        Task<FWTObjectID> ResolveAsync(StockportGovUK.NetStandard.Models.Verint.Organisation organisation);
    }
}
