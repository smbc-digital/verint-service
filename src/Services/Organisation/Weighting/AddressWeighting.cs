using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using VerintWebService;

namespace verint_service.Services.Organisation.Weighting
{
    public class AddressWeighting : IOrganisationWeighting
    {
        ILogger<AddressWeighting> _logger;

        public AddressWeighting(ILogger<AddressWeighting> logger)
        {
            _logger = logger;
        }
        
        public int Calculate(FWTOrganisation organisationObject, StockportGovUK.NetStandard.Models.Verint.Organisation organisation)
        {
            if(organisation.Address == null || 
                organisationObject.ContactPostals == null ||
                !string.IsNullOrEmpty(organisation.Address.UPRN))
            {
                _logger.LogDebug($"AddressWeighting.Calculate - No contact postals or address null or uprn is not empty - Returning 0 - {organisation.Name}");
                return 0;
            }

            var hasMatchingPostcode = false;
            var hasMatchingNumber = false;

            // This is effectively the same as a UPRN match (i.e. House Number and Postcode must match to be given weight)
            if (!string.IsNullOrEmpty(organisation.Address.Postcode) &&    
                organisationObject.ContactPostals.Any(x => !string.IsNullOrEmpty(x.Postcode) && 
                string.Equals(x.Postcode.Trim().Replace(" ", string.Empty), organisation.Address.Postcode.Trim().Replace(" ", string.Empty), StringComparison.CurrentCultureIgnoreCase)))
            {
                _logger.LogDebug($"AddressWeighting.Calculate - IS matching postcode {organisation.Address.Postcode}, {organisation.Name}");
                hasMatchingPostcode = true;
            }
            else
            {
                _logger.LogDebug($"AddressWeighting.Calculate - NON matching postcode {organisation.Address.Postcode}, {organisation.Name}");
            }

            // if (!string.IsNullOrEmpty(organisation.Address.Number) &&
            //     organisationObject.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressNumber) && 
            //     string.Equals(x.AddressNumber.Trim(), organisation.Address.Number.ToString(), StringComparison.CurrentCultureIgnoreCase)))
            // {
            //     _logger.LogDebug($"AddressWeighting.Calculate - IS matching number {organisation.Address.Number}, {organisation.Name}");
            //     hasMatchingNumber = true;
            // }
            // else{
            //     _logger.LogDebug($"AddressWeighting.Calculate - NON matching number {organisation.Address.Number}, {organisation.Name}");
            // }

            if(hasMatchingPostcode)
            {
                _logger.LogDebug($"AddressWeighting.Calculate - IS Matching address (postcode, number) - Returning 1 - {organisation.Name}");
                return 1;
            }

            _logger.LogDebug($"AddressWeighting.Calculate - NON Matching address (postcode, number) - Returning 0 - {organisation.Name}");
            return 0;
        }
    }
}