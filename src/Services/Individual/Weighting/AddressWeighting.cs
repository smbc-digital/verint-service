using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

public class AddressWeighting : IIndividualWeighting
{

    ILogger<AddressWeighting> _logger;

    public AddressWeighting(ILogger<AddressWeighting> logger)
    {
        _logger = logger;
    }
    
    public int Calculate(FWTIndividual individual, Customer customer)
    {
        
        if(customer.Address == null)
        {
            _logger.LogInformation($"AddressWeighting.Calculate, Customer Address Null, Score 0, Customer: {customer.Surname}");
            return 0;
        }

        if(individual.ContactPostals == null)
        {
            _logger.LogInformation($"AddressWeighting.Calculate, ContactPostals Null, Score 0, Customer: {customer.Surname}");
            return 0;
        }

        if(!string.IsNullOrEmpty(customer.Address.UPRN))
        {
            _logger.LogInformation($"AddressWeighting.Calculate, Customer Address UPRN Null, Score 0, Customer: {customer.Surname}");
            return 0;
        }

        var score = 0; 

        // This is effectively the same as a UPRN match (i.e. House Number and Postcode must match to be given weight)
        if (!string.IsNullOrEmpty(customer.Address.Postcode) &&    
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.Postcode) && string.Equals(x.Postcode.Trim().Replace(" ", string.Empty), customer.Address.Postcode.Trim().Replace(" ", string.Empty), StringComparison.CurrentCultureIgnoreCase)) && 
            !string.IsNullOrEmpty(customer.Address.Number) &&
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressNumber) && string.Equals(x.AddressNumber.Trim(), customer.Address.Number.ToString(), StringComparison.CurrentCultureIgnoreCase))
            )
        {
            _logger.LogInformation($"AddressWeighting.Calculate, House Number & Postcode Match, Score 2, Customer: {customer.Surname}");
            score += 2;
        }

        if (!string.IsNullOrEmpty(customer.Address.AddressLine1) &&
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[0]) && string.Equals(x.AddressLine[0].Trim(), customer.Address.AddressLine1.ToString(), StringComparison.CurrentCultureIgnoreCase)))
        {
            _logger.LogInformation($"AddressWeighting.Calculate, AddressLine1 Match, Score 1, Customer: {customer.Surname}");
            score += 1;
        }

        if (!string.IsNullOrEmpty(customer.Address.AddressLine2) &&
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[1]) && string.Equals(x.AddressLine[1].Trim(), customer.Address.AddressLine2.ToString(), StringComparison.CurrentCultureIgnoreCase)))
        {
            _logger.LogInformation($"AddressWeighting.Calculate, AddressLine2 Match, Score 1, Customer: {customer.Surname}");
            score += 1;
        }

        if (!string.IsNullOrEmpty(customer.Address.AddressLine3) &&
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[2]) && string.Equals(x.AddressLine[2].Trim(), customer.Address.AddressLine3.ToString(), StringComparison.CurrentCultureIgnoreCase)))
        {
            _logger.LogInformation($"AddressWeighting.Calculate, AddressLine3 Match, Score 1, Customer: {customer.Surname}");
            score += 1;
        }

        if (!string.IsNullOrEmpty(customer.Address.City) &&
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.City) && string.Equals(x.City.Trim(), customer.Address.City.ToString(), StringComparison.CurrentCultureIgnoreCase)))
        {
            _logger.LogInformation($"AddressWeighting.Calculate, Address.City Match, Score 1, Customer: {customer.Surname}");
            score += 1;
        }
        
        _logger.LogInformation($"AddressWeighting.Calculate, Address.City Match, Score {score}, Customer: {customer.Surname}");
        return score > 2 ? 2 : score;
    }
}