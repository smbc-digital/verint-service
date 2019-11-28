using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Addresses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Models.Verint;
using verint_service.Helpers.VerintConnection;
using VerintWebService;

namespace verint_service.Services.Street
{
    public class StreetService : IStreetService
    {
        private readonly ILogger<StreetService> _logger;

        private readonly IVerintClient _verintConnection;

        public StreetService(IVerintConnection verint, ILogger<StreetService> logger)
        {
            _logger = logger;
            _verintConnection = verint.Client();
        }

        public async Task<IEnumerable<Models.Street>> SearchByStreetAsync(string reference)
        {
            var streetSearch = new FWTStreetSearch
            {
                 StreetName = reference 
            };

            return await DoStreetSearch(streetSearch);
        }

        private async Task<IEnumerable<Models.Street>> DoStreetSearch(FWTStreetSearch streetSearch)
        {
            var streetSearchResults = await _verintConnection.searchForStreetAsync(streetSearch);
            var streetResults = streetSearchResults.FWTObjectBriefDetailsList.Select(result => new Models.Street
            {
                USRN = result.ObjectID.ObjectReference[0],
                Description = result.ObjectDescription
            });

            return streetResults;
        }
    }
}