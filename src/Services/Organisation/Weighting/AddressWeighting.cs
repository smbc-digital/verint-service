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
                return 0;
            }

            // This is effectively the same as a UPRN match (i.e. House Number and Postcode must match to be given weight)
            if (!string.IsNullOrEmpty(organisation.Address.Postcode) &&    
                organisationObject.ContactPostals.Any(x => !string.IsNullOrEmpty(x.Postcode) && string.Equals(x.Postcode.Trim().Replace(" ", string.Empty), organisation.Address.Postcode.Trim().Replace(" ", string.Empty), StringComparison.CurrentCultureIgnoreCase)) && 
                !string.IsNullOrEmpty(organisation.Address.Number) &&
                organisationObject.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressNumber) && string.Equals(x.AddressNumber.Trim(), organisation.Address.Number.ToString(), StringComparison.CurrentCultureIgnoreCase))
                )
            {
                return 1;
            }

            return 0;

            // if (!string.IsNullOrEmpty(organisation.Address.AddressLine1) &&
            //     organisationObject.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[0]) && string.Equals(x.AddressLine[0].Trim(), organisation.Address.AddressLine1.ToString(), StringComparison.CurrentCultureIgnoreCase)))
            // {
            //     score += 1;
            // }

            // if (!string.IsNullOrEmpty(organisation.Address.AddressLine2) &&
            //     organisationObject.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[1]) && string.Equals(x.AddressLine[1].Trim(), organisation.Address.AddressLine2.ToString(), StringComparison.CurrentCultureIgnoreCase)))
            // {
            //     score += 1;
            // }

            // if (!string.IsNullOrEmpty(organisation.Address.AddressLine3) &&
            //     organisationObject.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[2]) && string.Equals(x.AddressLine[2].Trim(), organisation.Address.AddressLine3.ToString(), StringComparison.CurrentCultureIgnoreCase)))
            // {
            //     score += 1;
            // }

            // if (!string.IsNullOrEmpty(organisation.Address.City) &&
            //     organisationObject.ContactPostals.Any(x => !string.IsNullOrEmpty(x.City) && string.Equals(x.City.Trim(), organisation.Address.City.ToString(), StringComparison.CurrentCultureIgnoreCase)))
            // {
            //     score += 1;
            // }
            
            // return score > 2 ? 2 : score;
        }
    }
}