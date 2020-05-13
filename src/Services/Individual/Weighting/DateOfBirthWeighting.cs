using System;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

public class DateOfBirthWeighting : IIndividualWeighting
{
    ILogger<DateOfBirthWeighting> _logger;

    public DateOfBirthWeighting(ILogger<DateOfBirthWeighting> logger)
    {
        _logger = logger;
    }

    public int Calculate(FWTIndividual individual, Customer customer)
    {
        if(customer.DateOfBirth != null && customer.DateOfBirth != DateTime.MinValue)
        {
            if(individual.DateOfBirthSpecified && individual.DateOfBirth == customer.DateOfBirth)
            {
                _logger.LogInformation($"DateOfBirthWeighting.Calculate, Match, Score 2, Customer: {customer.Surname}");
                return 2;
            }
            else if(individual.DateOfBirthSpecified && individual.DateOfBirth != customer.DateOfBirth)
            {
                _logger.LogInformation($"DateOfBirthWeighting.Calculate, No Match, Score -10, Customer: {customer.Surname}");
                return -10;
            }
        }

        _logger.LogInformation($"DateOfBirthWeighting.Calculate, No Date for Comparison, Score 0, Customer: {customer.Surname}");
        return 0;
    }
}