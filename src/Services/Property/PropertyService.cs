using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.Models.Fostering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Services.Property
{
    public class PropertyService : IPropertyService
    {
        private readonly ILogger<PropertyService> _logger;

        private readonly IVerintClient _verintConnection;

        public PropertyService(IVerintConnection verint, ILogger<PropertyService> logger)
        {
            _logger = logger;
            _verintConnection = verint.Client();
        }

        public async Task<IEnumerable<AddressSearchResult>> SearchByPostcodeAsync(string postcode)
        {
            var propertySearch = new FWTPropertySearch
            {
                Postcode = postcode
            };

            return await DoPropertySearch(propertySearch);
        }

        public async Task<IEnumerable<AddressSearchResult>> SearchByStreetAsync(string street)
        {
            var streetSearch = new FWTPropertySearch
            {
                StreetName = street
            };

            return await DoPropertySearch(streetSearch);
        }

        private async Task<IEnumerable<AddressSearchResult>> DoPropertySearch(FWTPropertySearch propertySearch)
        {
            var propertySearchResults = await _verintConnection.searchForPropertyAsync(propertySearch);
            var addressResults = propertySearchResults.FWTObjectBriefDetailsList.Select(result => new AddressSearchResult
            {
                UniqueId = result.ObjectID.ObjectReference[0],
                Name = result.ObjectDescription
            });

            return addressResults;
        }
    }
}
