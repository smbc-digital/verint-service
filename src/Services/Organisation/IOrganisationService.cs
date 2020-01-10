using StockportGovUK.NetStandard.Models.Models.Verint.Lookup;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace verint_service.Services.Organisation
{
    public interface IOrganisationService
    {
        Task<IEnumerable<OrganisationSearchResult>> SearchByOrganisationAsync(string organisationName);
    }
}
