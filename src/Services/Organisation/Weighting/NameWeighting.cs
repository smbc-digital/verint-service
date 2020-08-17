using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Services.Organisation.Weighting
{
    public class NameWeighting : IOrganisationWeighting
    {
        ILogger<NameWeighting> _logger;

        public NameWeighting(ILogger<NameWeighting> logger)
        {
            _logger = logger;
        }

        public int Calculate(FWTOrganisation organisationObject, StockportGovUK.NetStandard.Models.Verint.Organisation organisation)
        {
                if (organisation.Name != null && organisationObject.Name.Any(x => organisation.Name == x.FullName))
                {
                    return 1;
                }

                return 0;
        }
    }
}