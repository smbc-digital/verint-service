using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Services.Organisation.Weighting
{
    public class TelephoneWeighting : IOrganisationWeighting
    {
        ILogger<TelephoneWeighting> _logger;

        public TelephoneWeighting(ILogger<TelephoneWeighting> logger)
        {
            _logger = logger;
        }

        public int Calculate(FWTOrganisation organisationObject, StockportGovUK.NetStandard.Models.Verint.Organisation organisation)
        {
            if (!string.IsNullOrEmpty(organisation.Telephone) && 
                organisationObject.ContactPhones != null)
            {
                if (organisationObject.ContactPhones.Any(x => x.Number.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()
                    == organisation.Telephone.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()))
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}