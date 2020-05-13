using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

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
        {
            _logger.LogInformation($"UprnWeighting.Calculate, No Address, Score 0, Customer: {customer.Surname}");
            return 0;
        }

        if(string.IsNullOrEmpty(customer.Address.UPRN))
        {
            _logger.LogInformation($"UprnWeighting.Calculate, No UPRN, Score 0, Customer: {customer.Surname}");
            return 0;
        }

        if (individual.ContactPostals.Any(x => x.UPRN == customer.Address.UPRN.Trim()))
        {
            _logger.LogInformation($"UprnWeighting.Calculate, UPRN Match, Score 2, Customer: {customer.Surname}");
            return 2;
        }

        _logger.LogInformation($"UprnWeighting.Calculate, No Match, Score 0, Customer: {customer.Surname}");
        return 0;
    }
}