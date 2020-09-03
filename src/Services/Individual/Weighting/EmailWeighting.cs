using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Services.Individual.Weighting
{
    public class EmailWeighting : IIndividualWeighting
    {
        ILogger<EmailWeighting> _logger;

        public EmailWeighting(ILogger<EmailWeighting> logger)
        {
            _logger = logger;
        }

        public int Calculate(FWTIndividual individual, Customer customer)
        {
            if (individual.ContactEmails == null || 
                individual.ContactEmails.Length == 0 || 
                string.IsNullOrEmpty(customer.Email))
            {
                return 0;
            } 
            
            if (individual.ContactEmails.Any(x => x.EmailAddress == customer.Email))
            {
                return 1;
            } 

            return 0;
        }
    }
}