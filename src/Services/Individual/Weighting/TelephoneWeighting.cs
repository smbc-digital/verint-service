using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Services.Individual.Weighting
{
    public class TelephoneWeighting : IIndividualWeighting
    {
        ILogger<TelephoneWeighting> _logger;

        public TelephoneWeighting(ILogger<TelephoneWeighting> logger)
        {
            _logger = logger;
        }

        public int Calculate(FWTIndividual individual, Customer customer)
        {
            if (!string.IsNullOrEmpty(customer.Telephone) && 
                individual.ContactPhones != null)
            {
                if (individual.ContactPhones.Any(x => x.Number.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()
                    == customer.Telephone.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()))
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}