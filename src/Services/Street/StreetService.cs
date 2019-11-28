using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Addresses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using verint_service.Controllers;
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

        public async Task<IEnumerable<StreetX>> SearchByStreetAsync(string reference)
        {
            var streetSearch = new FWTStreetSearch
            {
                 StreetName = reference 
            };

            return await DoStreetSearch(streetSearch);
        }

        private async Task<IEnumerable<StreetX>> DoStreetSearch(FWTStreetSearch streetSearch)
        {
            var streetSearchResults = await _verintConnection.searchForStreetAsync(streetSearch);
            var streetResults = streetSearchResults.FWTObjectBriefDetailsList.Select(result => new StreetX
            {
                USRN = result.ObjectID.ObjectReference[0],
                Description = result.ObjectDescription
            });

            return streetResults;
        }
    }
}