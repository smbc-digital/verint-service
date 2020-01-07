using System.Collections.Generic;
using System.Threading.Tasks;

namespace verint_service.Services.Organisation
{
    public interface IOrganisationService
    {
        Task<IEnumerable<Models.Organisation>> SearchByOrganisationAsync(string organisationName);
    }
}
