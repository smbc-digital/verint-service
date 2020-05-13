using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

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
                _logger.LogInformation($"TelephoneWeighting.Calculate, Match, Score 1, Customer: {customer.Surname}");
                return 1;
            }
        }

        _logger.LogInformation($"TelephoneWeighting.Calculate, No Match, Score 0, Customer: {customer.Surname}");
        return 0;
    }
}