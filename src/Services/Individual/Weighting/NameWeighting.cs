using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

public class NameWeighting : IIndividualWeighting
{
    ILogger<NameWeighting> _logger;

    public NameWeighting(ILogger<NameWeighting> logger)
    {
        _logger = logger;
    }

    public int Calculate(FWTIndividual individual, Customer customer)
    {
            var score = 0;

            if (individual.Name != null)
            {
                if (!string.IsNullOrEmpty(customer.Forename) &&
                    individual.Name.Any(x => x.Forename.Any()) &&
                    individual.Name.Any(x => string.Join(" ", x.Forename).ToUpper().Trim().Contains(customer.Forename.ToUpper().Trim())))
                {
                    _logger.LogInformation($"NameWeighting.Calculate, Forename Match, Score 1, Customer: {customer.Surname}");
                    score += 1;
                }

                // Penalise users that provide a non matching name
                if (!string.IsNullOrEmpty(customer.Forename) &&
                    individual.Name.Any(x => x.Forename.Any()) &&
                    individual.Name.Any(x => !string.Join(" ", x.Forename).ToUpper().Trim().Contains(customer.Forename.ToUpper().Trim())))
                {
                    _logger.LogInformation($"NameWeighting.Calculate, Forename No Match, Score -10, Customer: {customer.Surname}");
                    score -= 10;
                }


                if (!string.IsNullOrEmpty(customer.Surname) &&
                    individual.Name.Any(x => !string.IsNullOrEmpty(x.Surname)) &&
                    individual.Name.Any(x => string.Equals(x.Surname.Trim(),
                        customer.Surname.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    _logger.LogInformation($"NameWeighting.Calculate, Surname Match, Score 1, Customer: {customer.Surname}");
                    score += 1;
                }

                if (!string.IsNullOrEmpty(customer.Surname) &&
                    individual.Name.Any(x => !string.IsNullOrEmpty(x.Surname)) &&
                    individual.Name.Any(x => !string.Equals(x.Surname.Trim(),
                        customer.Surname.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    _logger.LogInformation($"NameWeighting.Calculate, Surname No Match, Score -10, Customer: {customer.Surname}");
                    score -= 10;
                }

                if (!string.IsNullOrEmpty(customer.Title) &&
                    individual.Name.Any(x => !string.IsNullOrEmpty(x.Title)) &&
                    individual.Name.Any(x =>
                        string.Equals(x.Title.Trim(), customer.Title.Trim(),
                            StringComparison.CurrentCultureIgnoreCase)))
                {
                    _logger.LogInformation($"NameWeighting.Calculate, Title Match, Score 1, Customer: {customer.Surname}");
                    score += 1;
                }

                if (!string.IsNullOrEmpty(customer.Initials) &&
                    individual.Name.Any(x => (x.Initials != null
                        ? x.Initials.Trim().ToUpper() == customer.Initials.Trim().ToUpper()
                        : string.Empty == customer.Initials.Trim().ToUpper())))
                {
                    _logger.LogInformation($"NameWeighting.Calculate, Initials Match, Score 1, Customer: {customer.Surname}");
                    score += 1;
                }
            }

            return score > 2 ? 2 : score;
    }
}