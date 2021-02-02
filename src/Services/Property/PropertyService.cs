using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Addresses;
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
                Number = result.FWTProperty.AddressNumber,
                USRN = result.FWTProperty.USRN,
                Easting = result.FWTProperty.GPSItmGeoCode,
                Northing = result.FWTProperty.GPSUtmGeoCode
            };

            return address;
        }

        public async Task<string> CheckUPRNForId(StockportGovUK.NetStandard.Models.Verint.Address address)
        {
            // HACK: Check whether UPRN provided is actually an ID and if so lookup the real UPRN
            // If it's a real ID it shouldn't return a property!
            if(!string.IsNullOrEmpty(address.UPRN))
            {
                try{
                    var propertyResult = await GetPropertyAsync(address.UPRN);
                    if(propertyResult != null)
                    {
                        return propertyResult.UPRN;
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogWarning($"PropertyService.CheckUPRNForId - Exception occurred searching for property, assuming UPRN {address.UPRN}", ex);            
                }
            }

            return address.UPRN;
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

        [Obsolete("This method will not be used in the future.")]
        public async Task<IEnumerable<StockportGovUK.NetStandard.Models.Verint.Address>> GetPropertiesAsync(string propertySearch)
        {
            var fWTPropertySearch = new FWTPropertySearch
            {
                Postcode = propertySearch
            };

            var propertySearchResults = await _verintConnection.searchForPropertyAsync(fWTPropertySearch);
            var addressResults = propertySearchResults.FWTObjectBriefDetailsList.Select(result => new AddressSearchResult
            {
                UniqueId = result.ObjectID.ObjectReference[0],
                Name = result.ObjectDescription
            });

            var addressList = new List<StockportGovUK.NetStandard.Models.Verint.Address>();            

            foreach (var address in addressResults)
            {
                var fWTObjectID = new FWTObjectID
                {
                    ObjectReference = new[] { address.UniqueId },
                    ObjectType = VerintConstants.PropertyObjectType
                };

                var result = await _verintConnection.retrievePropertyAsync(fWTObjectID);

                addressList.Add(new StockportGovUK.NetStandard.Models.Verint.Address
                {
                    UPRN = result.FWTProperty.UPRN?.Trim(),
                    Description = address.Name?.Trim(),
                    AddressLine1 = result.FWTProperty.AddressLine1?.Trim(),
                    AddressLine2 = result.FWTProperty.AddressLine2?.Trim(),
                    City = result.FWTProperty.City?.Trim(),
                    Postcode = result.FWTProperty.Postcode?.Trim(),
                    Number = result.FWTProperty.AddressNumber?.Trim(),
                    USRN = result.FWTProperty.USRN?.Trim(),
                    Easting = result.FWTProperty.GPSItmGeoCode?.Trim(),
                    Northing = result.FWTProperty.GPSUtmGeoCode?.Trim()
                });
            }

            return addressList;
        }
    }
}