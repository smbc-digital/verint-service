using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Services.Individual.Weighting
{
    public class UprnWeighting : IIndividualWeighting
    {
        ILogger<UprnWeighting> _logger;

        public UprnWeighting(ILogger<UprnWeighting> logger)
        {
            _logger = logger;
        }

        public int Calculate(FWTIndividual individual, Customer customer)
        {
            if(customer.Address == null || individual.ContactPostals == null)
                return 0;

            if(string.IsNullOrEmpty(customer.Address.UPRN))
                return 0;

            if (individual.ContactPostals.Any(x => x.UPRN == customer.Address.UPRN.Trim()))
                return 1;

            return 0;
        }
    }
}