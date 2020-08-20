using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Models.Addresses;
using verint_service.Helpers.VerintConnection;
using verint_service.Utils.Consts;
using VerintWebService;

namespace verint_service.Services.Street
{
    public class StreetService : IStreetService
    {
        private readonly IVerintClient _verintConnection;

        public StreetService(IVerintConnection verint)
        {
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
                Name = result.BriefDetails?.ObjectDescription,
                AddressLine1 = JsonConvert.SerializeObject(result)
            };
        }

        public async Task<IEnumerable<AddressSearchResult>> SearchByStreetAsync(string reference) => await DoStreetSearch(new FWTStreetSearch { StreetName = reference });

        public async Task<IEnumerable<AddressSearchResult>> SearchByUsrnAsync(string usrn) => await DoStreetSearch(new FWTStreetSearch { USRN = usrn });

        private async Task<IEnumerable<AddressSearchResult>> DoStreetSearch(FWTStreetSearch streetSearch)
        {
            var streetResult = await GetUniqueId(streetSearch);
            return streetResult.Select(_ => GetStreet(_.UniqueId).Result);
        }

        private async Task<IEnumerable<AddressSearchResult>> GetUniqueId(FWTStreetSearch streetSearch)
        {
            var streetSearchResults = await _verintConnection.searchForStreetAsync(streetSearch);
            return streetSearchResults.FWTObjectBriefDetailsList.OrderBy(street => street.ObjectDescription).Select(result => new AddressSearchResult
            {
                UniqueId = result.ObjectID.ObjectReference[0],
                Name = result.ObjectDescription,
                AddressLine1 = JsonConvert.SerializeObject(result)
            });
        }
    }
}