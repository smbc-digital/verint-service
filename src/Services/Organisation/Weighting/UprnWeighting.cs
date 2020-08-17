using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Services.Organisation.Weighting
{
    public class UprnWeighting : IOrganisationWeighting
    {
        ILogger<UprnWeighting> _logger;

        public UprnWeighting(ILogger<UprnWeighting> logger)
        {
            _logger = logger;
        }

        public int Calculate(FWTOrganisation organisationObject, StockportGovUK.NetStandard.Models.Verint.Organisation organisation)
        {
            if(organisation.Address == null || organisationObject.ContactPostals == null)
            {
                return 0;
            }

            if(string.IsNullOrEmpty(organisation.Address.UPRN))
            {
                return 0;
            }

            if (organisationObject.ContactPostals.Any(x => x.UPRN == organisation.Address.UPRN.Trim()))
            {
                return 1;
            }

            return 0;
        }
    }
}