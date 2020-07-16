using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Addresses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using verint_service.Utils.Consts;
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

        public async Task<AddressSearchResult> GetStreet(string reference)
        {
            var result = (await _verintConnection.retrieveStreetAsync(new FWTObjectID
            {
                ObjectReference = new[] { reference },
                ObjectType = VerintConstants.StreetObjectType
            })).FWTStreet;

            return new AddressSearchResult
            {
                UniqueId = reference,
                USRN = result.USRN,
                Name = result.BriefDetails?.ObjectDescription
            };
        }

        public async Task<IEnumerable<AddressSearchResult>> SearchByStreetAsync(string reference)
        {
            var streetSearch = new FWTStreetSearch
            {
                 StreetName = reference 
            };

            return await DoStreetSearch(streetSearch);
        }

        private async Task<IEnumerable<AddressSearchResult>> DoStreetSearch(FWTStreetSearch streetSearch)
        {
            var streetSearchResults = await _verintConnection.searchForStreetAsync(streetSearch);
            var streetResults = streetSearchResults.FWTObjectBriefDetailsList.OrderBy(street => street.ObjectDescription).Select(result => new AddressSearchResult
            {
                UniqueId = result.ObjectID.ObjectReference[0],
                Name = result.ObjectDescription
            });

            return streetResults;
        }
    }
}