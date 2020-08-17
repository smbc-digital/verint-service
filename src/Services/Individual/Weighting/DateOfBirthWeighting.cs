using System;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Services.Individual.Weighting
{
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
                    return 2;
                }
                else if(individual.DateOfBirthSpecified && individual.DateOfBirth != customer.DateOfBirth)
                {
                    return -10;
                }
            }

            return 0;
        }
    }
}