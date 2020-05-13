using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

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
            _logger.LogInformation($"EmailWeighting.Calculate, No Match, Score 0, Customer: {customer.Surname}");
            return 0;
        } 
        
        if (individual.ContactEmails.Any(x => x.EmailAddress == customer.Email))
        {
            _logger.LogInformation($"EmailWeighting.Calculate, Match, Score 2, Customer: {customer.Surname}");
            return 2;
        } 

        return 0;
    }
}