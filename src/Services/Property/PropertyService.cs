using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.Verint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using verint_service.Utils.Consts;
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

        public async Task<StockportGovUK.NetStandard.Models.Verint.Address> GetPropertyAsync(string id)
        {
            var propertySearch = new FWTObjectID
            {
                ObjectReference = new [] { id },
                ObjectType = VerintConstants.PropertyObjectType
            };

            var result  = await _verintConnection.retrievePropertyAsync(propertySearch);

            var address = new StockportGovUK.NetStandard.Models.Verint.Address
            {
                UPRN = result.FWTProperty.UPRN,
                AddressLine1 = result.FWTProperty.AddressLine1,
                AddressLine2 = result.FWTProperty.AddressLine2,
                City = result.FWTProperty.City,
                Postcode = result.FWTProperty.Postcode,
                Number = result.FWTProperty.AddressNumber
            };

            return address;
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