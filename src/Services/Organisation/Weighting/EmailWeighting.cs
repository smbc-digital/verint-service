using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Services.Organisation.Weighting
{
    public class EmailWeighting : IOrganisationWeighting
    {
        ILogger<EmailWeighting> _logger;

        public EmailWeighting(ILogger<EmailWeighting> logger)
        {
            _logger = logger;
        }

        public int Calculate(FWTOrganisation organisationObject, StockportGovUK.NetStandard.Models.Verint.Organisation organisation)
        {
            if (organisationObject.ContactEmails == null || 
                organisationObject.ContactEmails.Length == 0 || 
                string.IsNullOrEmpty(organisation.Email))
            {
                return 0;
            } 
            
            if (organisationObject.ContactEmails.Any(x => x.EmailAddress == organisation.Email))
            {
                return 1;
            } 

            return 0;
        }
    }
}