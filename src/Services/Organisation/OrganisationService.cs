using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using verint_service.Helpers.VerintConnection;
using VerintWebService;

namespace verint_service.Services.Organisation
{
    public class OrganisationService : IOrganisationService
    {
        private readonly ILogger<OrganisationService> _logger;
        private readonly IVerintClient _verintConnection;

        public OrganisationService(IVerintConnection verint, ILogger<OrganisationService> logger)
        {
            _logger = logger;
            _verintConnection = verint.Client();
        }

        public async Task<IEnumerable<Models.Organisation>> SearchByOrganisationAsync(string organisationName)
        {
            var orgSearch = new FWTPartySearch
            {
                SearchType = "organisation",
                Name = organisationName
            };

            return await DoOrganisationSearch(orgSearch);
        }

        private async Task<IEnumerable<Models.Organisation>> DoOrganisationSearch(FWTPartySearch orgSearch)
        {
            var orgSearchResults = await _verintConnection.searchForPartyAsync(orgSearch);
            var orgResults = orgSearchResults.FWTObjectBriefDetailsList.Select(result => new Models.Organisation
            {
                Reference = result.ObjectID.ObjectReference[0],       
                Name = result.ObjectDescription
            });

            return orgResults;
        }
    }
}
